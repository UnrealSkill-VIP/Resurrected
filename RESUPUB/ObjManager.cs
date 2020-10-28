using RESUPUB.EngineObjects;
using RESUPUB.Helpers;
using RESUPUB.Off;
using RESUPUB.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RESUPUB
{
    public class ObjManager : EngineObjects.Object
    {
        //private readonly ILog _log = Log.Get();
        // Exposed through a read-only list, users of the API won't be able to change what's going on in game anyway.
        private readonly List<Entity> _players = new List<Entity>();

        public int gSize = 0;


        private readonly int _ticksPerSecond;
        public TimeSpan _lastUpdate = TimeSpan.Zero;

        public GlowObjDef[] gAllObject;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObjectManager" /> class.
        /// </summary>
        /// <param name="baseAddress">The base address.</param>
        /// <param name="capacity">The capacity.</param>
        /// <param name="ticksPerSecond">The ticks per second.</param>
        public ObjManager(IntPtr baseAddress, int capacity, int ticksPerSecond = 15) : base(baseAddress)
        {
            _ticksPerSecond = ticksPerSecond;

        }

        /// <summary>
        ///     Gets the current valid objects in the game world.
        /// </summary>
        public IReadOnlyList<Entity> Players => _players.Where(p => p.IsValid).ToList();

        internal Local LocalPlayer { get; private set; }

        /// <summary>
        ///     Updates the ObjectManager, obtaining all player entities from the game and adding them to the Players list.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        ///     Can not update the ObjectManager when it's not properly initialized!
        ///     Are you sure BaseAddress is valid?
        /// </exception>
        public void Update()
        {
            if (!IsValid)
                throw new InvalidOperationException(
                    "Can not update the ObjectManager when it's not properly initialized! Are you sure BaseAddress is valid?");

            var timeStamp = sTimer.GetTime();
            // Throttle the updates a little - entities won't be changing that frequently.
            // Realistically we don't need to call this very often at all, as we only keep references to the actual
            // entities in the game, and only resolve their members when they're actually required.
            if (timeStamp - _lastUpdate < TimeSpan.FromMilliseconds(1000 / _ticksPerSecond))
                return;

            if (!Manager.gClient.InGame)
            {
                // No point in updating if we're not in game - we'd end up reading garbage.
                // Do set the last update time though, we especially don't want to tick too often in menu.
                _lastUpdate = timeStamp;
                return;
            }

            // Prevent duplicate entries - more efficient would be maintaining a dictionary and updating entities.
            // Then again, this is significantly less code, and performance wise not too big an impact. Leave it be for now,
            // but consider updating this in the future.
            _players.Clear();

            var localPlayerPtr = Manager.Memory.Read<IntPtr>(Manager.ClientBase + (int)oBase.LocalPlayer);

            LocalPlayer = new Local(localPlayerPtr);

            var capacity = Manager.Memory.Read<int>(Manager.ClientBase + (int)oBase.EntityList + 0x4);
            for (var i = 0; i < capacity; i++)
            {
                _players.Add(new Entity(GetEntityPtr(i)));
            }
            //Wlog();
            //Trace.WriteLine($"[EntityManager] Update complete. {Players.Count(s => s.IsValid)} valid entries found.");

            

            _lastUpdate = timeStamp;
        }

        public int ReadInt(IntPtr addr)
        {
            return Manager.Memory.Read<int>(addr);
        }

        public void WriteBool(IntPtr addr, bool b)
        {
            Manager.Memory.Write<bool>(false, b, addr);
        }

        public void WriteFloat(IntPtr addr, float f)
        {
            Manager.Memory.Write<float>(false, f, addr);
        }

        public void WriteBytes(IntPtr addr, byte[] bytez)
        {
            Manager.Memory.WriteBytes(addr, bytez);
        }

        private IntPtr GetEntityPtr(int index)
        {
            // ptr = entityList + (idx * size)
            return Manager.Memory.Read<IntPtr>(BaseAddress + index * (int)oStatic.EntitySize);
        }

        /// <summary>
        ///     Gets the player with the specified ID, and null if that player doesn't exist.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Entity GetPlayerById(int id)
        {
            if (_players.Count < id)
                return null;

            return Players.FirstOrDefault(p => p.Id == id);
        }


        public void WriteGlowObject(GlowObjDef def, int index)
        {
            byte[] data = def.GetBytes();
            byte[] writeData = new byte[GlowObjDef.GetSize() - 14];
            Array.Copy(data, 4, writeData, 0, writeData.Length);
            Manager.Memory.WriteBytes(Manager.ClientBase + GlowObjDef.GetSize() * index + 4, writeData);
        }
        public static T GetStructure<T>(byte[] data)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            T structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return structure;
        }
    }
}
