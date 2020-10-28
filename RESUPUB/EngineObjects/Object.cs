using RESUPUB.Helpers;
using RESUPUB.Off;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RESUPUB.EngineObjects
{
    public abstract class Object : IEquatable<Object>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NativeObject" /> class.
        /// </summary>
        /// <param name="baseAddress">The base address.</param>
        protected Object(IntPtr baseAddress)
        {
            BaseAddress = baseAddress;
        }

        /// <summary>
        ///     Gets the base address of this object in the remote process.
        /// </summary>
        public IntPtr BaseAddress { get; }

        /// <summary>
        ///     Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsValid => BaseAddress != IntPtr.Zero;

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return BaseAddress.GetHashCode();
        }

        public SharpDX.Vector3 ReadAngle(int addr)
        {
            byte[] data = Manager.Memory.ReadBytes(Manager.EnginePointer + addr, 12);
            return new SharpDX.Vector3(BitConverter.ToSingle(data, 0), BitConverter.ToSingle(data, 4), BitConverter.ToSingle(data, 8));
        }

        private static byte[] copyArray(SharpDX.Vector3 angles, byte[] toCopy)
        {
            Array.Copy(BitConverter.GetBytes(angles.X), 0, toCopy, 0, 4);
            Array.Copy(BitConverter.GetBytes(angles.Y), 0, toCopy, 4, 4);
            Array.Copy(BitConverter.GetBytes(angles.Z), 0, toCopy, 8, 4);
            return toCopy;
        }

        public void WriteAngles(SharpDX.Vector3 angles)
        {
            Manager.Memory.WriteBytes(Manager.EnginePointer + (int)oStatic.ViewAngleX, copyArray(angles, new byte[12]));
        }

        public string ReadModelName(int off, bool isalive)
        {
            try
            {
                if (isalive)
                {
                    int modelPnt = Manager.Memory.Read<int>((IntPtr)off + 0x6C);
                    return Manager.Memory.ReadString((IntPtr)modelPnt + 0x4, Encoding.UTF8);
                }
                return "notFound";
            } catch { return "notFound"; }
            

        }

        public int iRanking(int index)
        {
            try
            {
                IntPtr rpoint = Manager.Memory.Read<IntPtr>(Manager.ClientBase + (int)oBase.PlayerResource);
                return Manager.Memory.Read<int>(rpoint + (int)oStatic.ICompRank + (int)(index * 4));
            }
            catch { return 0; }
            
        }

        public int iKills(int index)
        {
            try
            {
                
                return Manager.Memory.Read<int>(Manager.ResourcePointer + 0xBE8 + (int)(index * 4));
            }
            catch { return 0; }
        }

        public int iRankWins(int index)
        {
            //ResourcePointer + ICompWinsOff + EntID * 4
            try
            {
                IntPtr rpoint = Manager.Memory.Read<IntPtr>(Manager.ClientBase + (int)oBase.PlayerResource);
                return Manager.Memory.Read<int>(rpoint + (int)oStatic.ICompWins + (int)(index * 4));
            }
            catch { return 0; }
            
        }

        public int i_isSpotted(int off)
        {
            try
            {
                return Manager.Memory.Read<int>(BaseAddress + off) & (int)oStatic.SpottedMask;
            }
            catch { return 0; }
            
        }

        //public void WriteGlowObject(GlowObjDef def, int index)
        //{
        //    byte[] data = def.GetBytes();
        //    byte[] writeData = new byte[def.GetSize() - 14];
        //    Array.Copy(data, 4, writeData, 0, writeData.Length);
        //    int glowAddr = WinAPI.ReadInt32(scanner.Process.Handle, dllClientAddress + GameOffsets.CL_GLOWMANAGER);
        //    WinAPI.WriteMemory(scanner.Process.Handle, glowAddr + GlowObjectDefinition.GetSize() * index + 4, writeData, writeData.Length);
        //}
        public string ReadName(int offset, int id, bool isAlive)
        {
            try
            {
                if (isAlive)
                    return Manager.Memory.ReadString(Manager.RadarPointer + (id * (int)oStatic.RadarSize + 0x24), System.Text.Encoding.Unicode, 512);
                return "";
            } catch { Console.WriteLine(string.Format("ReadWeaponID Failed :( - Returning {0}", "nothing")); return ""; }
        }

        public int ReadSignOnState()
        {
            try
            {
                return Manager.Memory.Read<int>(Manager.EnginePointer + (int)oStatic.GameState);
            } catch { return 0; }
            
        }

        public int CreateInt(byte[] one, int two)
        {
            return BitConverter.ToInt32(one, two);
        }
        /// <summary>
        ///     Reads a member of the specified type at the specified offset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        /// 
        protected T ReadField<T>(int offset) where T : struct
        {
            try
            {
                return Manager.Memory.Read<T>(BaseAddress + offset);
            }
            catch { Console.WriteLine(string.Format("ReadField Failed :( - Returning {0}", default(T).ToString())); return default(T); }
            // We don't check for IsValid here because reading even on an invalid object would
            // not directly be an invalid operation - the read operation will throw an exception later down the line.


        }

        /// <summary>
        ///     Reads a member of the specified type at the specified offset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        protected T ReadField<T>(oStatic offset) where T : struct
        {
            return ReadField<T>((int)offset);
        }

        /// <summary>
        ///     Reads a member of the specified type at the specified offset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        protected T ReadField<T>(oBase offset) where T : struct
        {
            return ReadField<T>((int)offset);
        }

        public int ReadClassId()
        {
            try
            {
                return Manager.Memory.Read<int>((IntPtr)Manager.Memory.Read<int>((IntPtr)Manager.Memory.Read<int>((IntPtr)Manager.Memory.Read<int>(BaseAddress + 0x8) + 2 * 0x4) + 0x1) + 20);
            } catch { return 0; }
        }

        public int ReadWeaponId()
        {
            try
            {
                int wpHandle = Manager.Memory.Read<int>(this.BaseAddress + (int)oStatic.LocalActiveWeapon);
                int weaponIndex = wpHandle & 4095;
                //int uioff = ((weaponIndex - 1) * 0x10);
                int wpBase = Manager.Memory.Read<int>((Manager.ClientBase + (int)oBase.EntityList) + (weaponIndex - 1) * 0x10);
                return Manager.Memory.Read<int>((IntPtr)wpBase + (int)oStatic.WIP);
            }
            catch { Console.WriteLine(string.Format("ReadWeaponID Failed :( - Returning {0}", 0.ToString())); return 0; }
        }

        protected T ReadField<T>(int id, bool fa) where T : struct
        {
            //int wpHandle = Manager.Memory.Read<int>(Manager.Me.BaseAddress + (int)oStatic.LocalActiveWeapon);
            //int weaponIndex = wpHandle & 0xFFF;
            //int theEnt = Manager.Memory.Read<int>((IntPtr)oBase.EntityList + weaponIndex * 0x10);
            try
            {
                return Manager.Memory.Read<T>(Manager.Me.BaseAddress + (int)oStatic.LocalActiveWeapon)/* & 0xFFF*/;
            } catch { return default(T); }
            
        }
        protected T ReadField<T>(IntPtr addr, int id) where T : struct
        {
            try
            {
                int wpHandle = Manager.Memory.Read<int>(addr + (int)oStatic.LocalActiveWeapon);
                int weaponIndex = wpHandle & 0xFFF;
                int uioff = ((weaponIndex - 1) * 0x10);
                int wpBase = Manager.Memory.Read<int>(Manager.ClientBase + (int)oBase.EntityList + uioff);
                return Manager.Memory.Read<T>((IntPtr)wpBase + (int)oStatic.WIP);
            } catch { Console.WriteLine(string.Format("ReadWeaponID Failed :( - Returning {0}", default(T).ToString())); return default(T); }

        }

        #region Implementation of IEquatable

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Object other)
        {
            return BaseAddress == other.BaseAddress;
        }

        public Matrix4x4 ReadMatrix(oBase offset)
        {
            Matrix4x4 mymatrix = new Helpers.Matrix4x4();
            
            return (mymatrix.ReadMatrix(Manager.ClientBase + (int)offset));
        }

        public Matrix3x4 theMatrix(int offset)
        {
            Matrix3x4 newmatrix = new Matrix3x4();
            return (newmatrix.ReadMatrix(this.BaseAddress + offset));
        }

        //public Hashtable GetBoneVec(IntPtr offset, string modelName, bool isAlive)
        //{
        //    try
        //    {
        //        if (isAlive)
        //        {
        //            Hashtable bonez = new Hashtable();
        //            int[] listOfBones = Boner.GetBonesByModel(modelName);
        //            for (int i = 0; i < listOfBones.Length; i++)
        //            {
        //                int boneBase = Manager.Memory.Read<int>(offset + (int)oStatic.Bonez);
        //                SharpDX.Vector3 rtnVec = new SharpDX.Vector3(
        //                    Manager.Memory.Read<float>((IntPtr)boneBase + 0x30 * listOfBones[i] + 0x0C),
        //                    Manager.Memory.Read<float>((IntPtr)boneBase + 0x30 * listOfBones[i] + 0x1C),
        //                    Manager.Memory.Read<float>((IntPtr)boneBase + 0x30 * listOfBones[i] + 0x2C)
        //                    );
        //                bonez.Add(i, rtnVec);
        //            }
        //            return bonez;
        //        }
        //        return new Hashtable();
        //    }
        //    catch { return new Hashtable(); }
        //}

        public List<SharpDX.Vector3> GetBoneVec(IntPtr offset, int bone, bool isalive, int amount)
        {
            try
            {
                if (isalive)
                {
                    List<SharpDX.Vector3> thisBonez = new List<SharpDX.Vector3>();
                    for (int i = 0; i < amount; i++)
                    {
                        int boneBase = Manager.Memory.Read<int>(offset + (int)oStatic.Bonez);
                        SharpDX.Vector3 rtnVec = new SharpDX.Vector3(
                            Manager.Memory.Read<float>((IntPtr)boneBase + 0x30 * i + 0x0C),
                            Manager.Memory.Read<float>((IntPtr)boneBase + 0x30 * i + 0x1C),
                            Manager.Memory.Read<float>((IntPtr)boneBase + 0x30 * i + 0x2C)
                            );
                        thisBonez.Add(rtnVec);
                    }

                    return thisBonez;
                }
                return new List<SharpDX.Vector3>();
            } catch { Console.WriteLine("Bone update failed :("); return new List<SharpDX.Vector3>(); }


        }
        public SharpDX.Vector3 GetBoneVec(IntPtr offset, int bone, bool isalive)
        {
            try
            {

                if (isalive)
                {
                    int boneBase = Manager.Memory.Read<int>(offset + (int)oStatic.Bonez);
                    SharpDX.Vector3 rtnVec = new SharpDX.Vector3(
                        Manager.Memory.Read<float>((IntPtr)boneBase + 0x30 * bone + 0x0C),
                        Manager.Memory.Read<float>((IntPtr)boneBase + 0x30 * bone + 0x1C),
                        Manager.Memory.Read<float>((IntPtr)boneBase + 0x30 * bone + 0x2C)
                        );
                    return rtnVec;
                }
                return new SharpDX.Vector3(0, 0, 0);
            } catch { Console.WriteLine("Bone update failed :("); Generators.Log(string.Format("Failed: {0} on bone {1}. Target was alive={2}", offset, bone, isalive), "RPMWPM.log"); return new SharpDX.Vector3(); }

        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Object)obj);
        }

        #endregion

        #region Operators

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(Object left, Object right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(Object left, Object right)
        {
            return !Equals(left, right);
        }

        public WeaponType GetWeaponType(int ID)
        {
            switch (ID)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 30:
                case 32:
                case 36:
                    return WeaponType.Pistol;
                case 7:
                case 10:
                case 13:
                case 16:
                    return WeaponType.AssaultRifle;
                case 8:
                case 39:
                    return WeaponType.ZoomRifle;
                case 9:
                case 40:
                    return WeaponType.Sniper;
                case 11:
                case 38:
                    return WeaponType.AutoSniper;
                case 14:
                case 28:
                    return WeaponType.MachineGun;
                case 17:
                case 19:
                case 24:
                case 26:
                case 33:
                    return WeaponType.MachinePistol;
                case 25:
                case 27:
                case 29:
                    return WeaponType.Shotgun;
                case 41:
                case 42:
                    return WeaponType.Melee;
                case 43:
                case 44:
                case 45:
                case 46:
                case 47:
                case 48:
                    return WeaponType.Grenade;
                case 31:
                case 49:
                    return WeaponType.Special;
                default:
                    return WeaponType.Unknown;
            }
        }
        #endregion
    }
}