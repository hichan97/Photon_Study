using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject _effect;

    private void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 1000.0f);
        //���� �ð��� ���� �� �Ѿ� ����
        Destroy(this.gameObject, 3.0f);
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("BULLET") == false)
        { 
            //�浹 ���� ����
            var _contact = coll.GetContact(0);
            //�浹 ������ ����Ʈ ����

        
            var _obj = Instantiate(_effect,
                               _contact.point,
                               Quaternion.LookRotation(-_contact.normal));
            Destroy(_obj, 2.0f);
            Destroy(this.gameObject);

        }

           
    }
}
