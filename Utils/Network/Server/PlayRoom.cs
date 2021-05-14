using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public enum GameProgress { ReadyGame = 0, ReadyRound, PlayRound };
    public enum UserType { Offender = 0, Defender };
    class PlayRoom
    {
        int _roomId;
        int _defenderId;
        int _offenderId;
        GameProgress currentProgress;

        public PlayRoom(int roomId, int defenderId, int offenderId)
        {
            _roomId = roomId;
            _defenderId = defenderId;
            _offenderId = offenderId;
        }
    }
}
