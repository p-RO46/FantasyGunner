using UnityEngine;

[System.Serializable]
public class Weapon
{
    public Weapon(WeaponData w)
    {
        _weaponData = w;
        _maximumBullet = w.maxAmmo;
        _loadedBullet = w.loadAmmo;
        _remainBullet = _maximumBullet - _loadedBullet;
    }

    public WeaponData _weaponData;
    private int _maximumBullet; // 총 살때 기본으로 들어있는 총알
    private int _remainBullet;
    private int _loadedBullet;
    public bool hasBullet()
    {
        return _remainBullet + _loadedBullet > 0;
    }

    public WeaponData weaponData
    {
        get { return _weaponData; }
        set 
        { 
            _weaponData = value;
        }
    }

    public int maximumBullet
    {
        get { return _maximumBullet; }
        set 
        { 
            _maximumBullet = value;
        }
    }

    public int loadedBullet
    {
        get { return _loadedBullet; }
        set 
        { 
            _loadedBullet = value;
        }
    }

    public int remainBullet
    {
        get { return _remainBullet; }
        set 
        { 
            _remainBullet = value;
        }
    }
}
