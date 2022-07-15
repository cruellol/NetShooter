using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Net
{
    public class ProjctileController : MonoBehaviour
    {
        [SerializeField]
        private float _moveSpeed=3f;
        [SerializeField]
        private float _damage = 1f;
        [SerializeField]
        private float _lifeTime = 7f;
        public float GetDamage => _damage;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(OnDie());
        }

        // Update is called once per frame
        void Update()
        {
            transform.position += transform.forward * _moveSpeed * Time.deltaTime;
        }

        private IEnumerator OnDie()
        {
            yield return new WaitForSeconds(_lifeTime);
            Destroy(gameObject);
        }
    } 
}
