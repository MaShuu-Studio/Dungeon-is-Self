using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameControl;

namespace Network
{
    public class NetworkGameController : MonoBehaviour
    {
        #region Instance
        private static NetworkGameController instance;
        public static NetworkGameController Instance
        {
            get
            {
                var obj = FindObjectOfType<NetworkGameController>();
                instance = obj;
                return instance;
            }
        }
        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        private int roomId = -1;
        public GameProgress currentProgress { get; private set; }
        public UserType userType { get; private set; }

        public void StartGame(int id, UserType type)
        {
            roomId = id;
            userType = type;
            SceneController.Instance.ChangeScene("NetworkGamePlay");
        }
    }
}
