using Photon.Pun;

namespace Net.Managers
{
    public class MenuManager : MonoBehaviourPunCallbacks
    {

        public void OnCreateRoom_UnityEditor()
        {
            //if (PhotonNetwork.IsMasterClient)
            //{
            //    Debugger.Log("Connected");
            //}
            PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
        }
        public void OnJoinRoom_UnityEditor()
        {
            Debugger.Log("Room count " + PhotonNetwork.CountOfRooms);
            PhotonNetwork.JoinRandomRoom();
        }
        public void OnQuitRoom_UnityEditor()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
            //Application.Quit();
#endif
        }

        // Start is called before the first frame update
        void Start()
        {
#if UNITY_EDITOR
            PhotonNetwork.NickName = "1";
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
            PhotonNetwork.NickName = "2";
#endif

            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = "0.0.7";
            PhotonNetwork.ConnectUsingSettings();
        }
        public override void OnConnectedToMaster()
        {
            Debugger.Log("Ready for connection");
        }

        public override void OnJoinedRoom()
        {
            PhotonNetwork.LoadLevel("NetGameScene");
        }
    }
}
