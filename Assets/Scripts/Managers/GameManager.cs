using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Net.Managers
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        private PlayerControls _player1;
        private PlayerControls _player2;

        [SerializeField]
        private string _playerPrefabName;
        [SerializeField]
        private InputAction _quit;
        [SerializeField]
        private float _randomInterval = 7f;

        // Start is called before the first frame update
        void Start()
        {
            _quit.Enable();
            _quit.performed += OnQuit;
            Vector3 pos = Vector3.zero;
            if (PhotonNetwork.NickName.Contains("1"))
            {
                pos = new Vector3(Random.Range(-_randomInterval, 0), 0f, Random.Range(-_randomInterval, _randomInterval));
            }
            else
            {
                pos = new Vector3(Random.Range(0, _randomInterval), 0f, Random.Range(-_randomInterval, _randomInterval));
            }
             
            GameObject GO = PhotonNetwork.Instantiate(_playerPrefabName+PhotonNetwork.NickName, pos, new Quaternion());

            PhotonPeer.RegisterType(typeof(PlayerData), 100,PlayerData.SerializePlayerData,PlayerData.DeserializePlayerData);
        }

        public void AddPlayer(PlayerControls player)
        {
            if (player.name.Contains("1"))
            {
                _player1 = player;
            }
            else
            {
                _player2 = player;
            }

            if (_player1 != null && _player2 != null)
            {
                _player1.SetTarget(_player2.transform);
                _player2.SetTarget(_player1.transform);
            }
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        private void OnQuit(InputAction.CallbackContext obj)
        {
            PhotonNetwork.LeaveRoom();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
            Application.Quit();
#endif
        }

        private void OnDestroy()
        {
            _quit.Dispose();
        }

        // Update is called once per frame
        void Update()
        {

        }
    } 
}
