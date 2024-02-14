using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManager : MonoBehaviour
{
    public static BulletPoolManager instance; //½Ì±ÛÅæ

    [SerializeField] private GameObject bulletPrefab; //ÃÑ¾ËÇÁ¸®ÆÕ

    private int maxBullets = 40; //ÃÑ¾Ë°¹¼ö

    private List<GameObject> bulletPool = new List<GameObject>(); //ÃÑ¾Ë ¸®½ºÆ®

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
            GameObject bulletGo = Instantiate(this.bulletPrefab); //ÃÑ¾Ë »ý¼º
            bulletGo.transform.SetParent(this.transform); //ÃÑ¾Ë À§Ä¡
            bulletGo.SetActive(false); //ÃÑ¾Ë ºñÈ°¼ºÈ­
            this.bulletPool.Add(bulletGo); //¸®½ºÆ®¿¡ Ãß°¡
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
