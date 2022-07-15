using Net.Managers;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

namespace Net
{
    public class PlayerControls : MonoBehaviour, IPunObservable
    {
        private Controls _controls;
        private Transform _bulletPool;
        private Camera _camera;
        private float _cameraYShift = 7;
        private float _cameraZShift = -6;
        private PanelScript _winLoosePanel;

        [SerializeField]
        private float _moveSpeed = 2f;
        [SerializeField]
        private float _maxSpeed = 2f;
        [SerializeField]
        private float _attackDelay = 0.5f;
        [SerializeField]
        private float _rotateDelay = 0.5f;
        [SerializeField]
        private Rigidbody _rigidBody;
        [SerializeField]
        private CapsuleCollider _collider;
        
        private Transform _target;
        [SerializeField]
        private ProjctileController _bulletPrefab;
        [SerializeField]
        private Vector3 _firePoint;
        [SerializeField]
        private PhotonView _photonView;

        public float Health = 20f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_firePoint, 0.2f);
        }

        void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _collider = GetComponent<CapsuleCollider>();
            _bulletPool = FindObjectOfType<UnityEngine.EventSystems.EventSystem>().transform;

            _controls = new Controls();

            FindObjectOfType<GameManager>().AddPlayer(this);
            if (_photonView.IsMine)
            {
                _camera = GameObject.FindObjectOfType<Camera>();
                _winLoosePanel=GameObject.FindObjectOfType<PanelScript>();
                _winLoosePanel.gameObject.SetActive(false);
            }
        }

        internal void SetTarget(Transform transform)
        {
            _target = transform;
            if (!_photonView.IsMine) return;


            _controls.Player1.Enable();

            StartCoroutine(Fire());
            StartCoroutine(Focus());
        }

        private IEnumerator Fire()
        {
            while (true)
            {
                yield return new WaitForSeconds(_attackDelay);
                //var bullet = Instantiate(_bulletPrefab, _bulletPool);
                //bullet.transform.position = transform.TransformPoint(_firePoint);
                //bullet.transform.rotation = transform.rotation;
                PhotonNetwork.Instantiate("Bullet", transform.TransformPoint(_firePoint), transform.rotation);
            }
        }
        private IEnumerator Focus()
        {
            while (true)
            {
                transform.LookAt(_target);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                yield return new WaitForSeconds(_rotateDelay);
            }
        }

        void FixedUpdate()
        {
            if (!_photonView.IsMine) return;

            Vector3 pos = transform.position;
            pos.y += _cameraYShift;
            pos.z += _cameraZShift;
            _camera.transform.position = pos;
            

            var direction = _controls.Player1.Movement.ReadValue<Vector2>();
            if (direction.x == 0 && direction.y == 0) return;

            var velocity = _rigidBody.velocity;
            velocity += new Vector3(direction.x, 0f, direction.y) * _moveSpeed * Time.fixedDeltaTime;
            velocity.y = 0f;
            velocity = Vector3.ClampMagnitude(velocity, _maxSpeed);
            _rigidBody.velocity = velocity;
        }

        private void OnDestroy()
        {
            _controls.Player1.Disable();
            _controls.Player2.Disable();
        }

        private void OnTriggerEnter(Collider other)
        {
            var bullet = other.GetComponent<ProjctileController>();
            var edge = other.GetComponent<EdgesControl>();
            if (bullet != null)
            {
                Health -= bullet.GetDamage;
                Destroy(other.gameObject);
                if (Health <= 0f)
                {
                    DeadPlayer();
                }
            }
            else if(edge!=null)
            {
                Health = 0;
                DeadPlayer();
            }            
        }

        private void DeadPlayer()
        {
            if (!_photonView.IsMine)
            {
                _target.GetComponent<PlayerControls>().SetWinner();                
            }
            else
            {
                StartCoroutine(AfterDeath(false));
            }
        }

        private void SetWinner()
        {
            StartCoroutine(AfterDeath(true));
        }

        private IEnumerator AfterDeath(bool win)
        {
            _controls.Player1.Disable();
            _controls.Player2.Disable();
            _rigidBody.velocity = Vector3.zero;
            StopAllCoroutines();
            if (win)
            {
                _winLoosePanel.ShowAndSetText("Победа");
                Health = 1000;
            }
            else
            {
                _winLoosePanel.ShowAndSetText("Потрачено");
            }
            yield return new WaitForSeconds(3);
            PhotonNetwork.LoadLevel("NewMenuScene");
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(PlayerData.Create(this));
            }
            else
            {
                ((PlayerData)stream.ReceiveNext()).Set(this);
                if (Health <= 0f)
                {
                    DeadPlayer();
                }
            }
        }
    }
}
