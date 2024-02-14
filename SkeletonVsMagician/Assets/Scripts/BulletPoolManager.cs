using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManager : MonoBehaviour
{
    public static BulletPoolManager instance; //�̱���

    [SerializeField] private GameObject bulletPrefab; //�Ѿ�������

    private int maxBullets = 40; //�Ѿ˰���

    private List<GameObject> bulletPool = new List<GameObject>(); //�Ѿ� ����Ʈ

    private void Awake()
    {
        if(BulletPoolManager.instance == null)
        {
            BulletPoolManager.instance = this;
        }    
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < this.maxBullets; i++)
        {
            GameObject bulletGo = Instantiate(this.bulletPrefab); //�Ѿ� ����
            bulletGo.transform.SetParent(this.transform); //�Ѿ� ��ġ
            bulletGo.SetActive(false); //�Ѿ� ��Ȱ��ȭ
            this.bulletPool.Add(bulletGo); //����Ʈ�� �߰�
        }
    }

    public GameObject GetBullet()
    {
        foreach(GameObject bullet in this.bulletPool)
        {
            if(bullet.activeSelf == false)
            {
                bullet.transform.SetParent(null);
                return bullet;
            }
        }
        return null;
    }

    public void Release(GameObject bullet)
    {
        bullet.SetActive(false);
        bullet.transform.SetParent(this.transform);
    }
}
