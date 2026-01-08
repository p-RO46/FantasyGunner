using UnityEngine;

public class Weapon
{
    public Weapon(WeaponData w)
    {
        _weaponData = w;
        _maximumBullet = w.maxAmmo;
        _loadedBullet = w.loadAmmo;
        _remainBullet = _maximumBullet - _loadedBullet;
        hasBullet = true;
    }

    private WeaponData _weaponData;
    private int _maximumBullet;
    private int _remainBullet;
    private int _loadedBullet;
    public bool hasBullet { get; set; }

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
