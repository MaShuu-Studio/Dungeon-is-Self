using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public enum GameProgress { ReadyGame = 0, ReadyRound, PlayRound };
    public enum UserType { Offender = 0, Defender };
    class PlayRoom
    {
        GameRoom _room;
        public int RoomId { get { return _roomId; } }

        int _roomId;

        int _defenderId;
        int _offenderId;

        Offender offender;
        Defender defender;

        GameProgress currentProgress;

        bool defenderIsReady = false;
        bool offenderIsReady = false;

        int round = 0;

        public PlayRoom(int roomId, int defenderId, int offenderId, GameRoom room)
        {
            _roomId = roomId;
            _defenderId = defenderId;
            _offenderId = offenderId;

            currentProgress = GameProgress.ReadyGame;
            offender = new Offender();
            defender = new Defender();

            round = 0;

            _room = room;
        }
        public PlayRoom(int roomId, int playerId, UserType type, GameRoom room)
        {
            _roomId = roomId;
            if (type == UserType.Defender)
            {
                _defenderId = playerId;
                _offenderId = -1;
                offenderIsReady = true;
            }
            else
            {
                _offenderId = playerId;
                _defenderId = -1;
                defenderIsReady = true;
            }
            currentProgress = GameProgress.ReadyGame;
            offender = new Offender();
            defender = new Defender();
            round = 0;

            _room = room;
        }

        public void ReadyGameEnd(UserType type, List<int> candidates)
        {
            if (type == UserType.Defender)
            {
                defender.SetCandidate(candidates);
                defenderIsReady = true;
            }
            else
            {
                offender.SetCandidate(candidates);
                offenderIsReady = true;
            }

            if (defenderIsReady && offenderIsReady)
            {
                currentProgress = GameProgress.ReadyRound;
                S_ReadyGameEnd packet = new S_ReadyGameEnd();
                packet.currentProgress = (ushort)currentProgress;
                packet.round = ++round;
                packet.enemyCandidates = defender.Candidates;

                _room.Send(_offenderId, packet);

                packet.enemyCandidates = offender.Candidates;
                _room.Send(_defenderId, packet);

                offenderIsReady = false;
                defenderIsReady = false;
            }
        }
    }
}
