using RESUPUB.EngineObjects;
using RESUPUB.Off;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESUPUB
{
    public class Game : RESUPUB.EngineObjects.Object
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameClient" /> class.
        /// </summary>
        /// <param name="baseAddress">The base address.</param>
        public Game(IntPtr baseAddress) : base(baseAddress)
        {
            //_log.Info("GameClient initialized.");
        }

        /// <summary>
        ///     Gets the index of the local player.
        /// </summary>
        /// <value>
        ///     The index of the local player.
        /// </value>
        public int LocalPlayerIndex => ReadField<int>(oStatic.LocalPlayerIndex);

        /// <summary>
        ///     Gets the state of the game client.
        /// </summary>
        /// <value>
        ///     The state.
        /// </value>
        public SignonState State => (SignonState)ReadSignOnState();//ReadField<int>(oStatic.GameState);

        /// <summary>
        ///     Gets a value indicating whether [in game].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [in game]; otherwise, <c>false</c>.
        /// </value>
        public bool InGame => State == SignonState.Full;

        /// <summary>
        ///     Gets a value indicating whether [in menu].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [in menu]; otherwise, <c>false</c>.
        /// </value>
        public bool InMenu => State == SignonState.None;
    }
}
