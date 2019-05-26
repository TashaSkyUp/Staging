using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum Weapon_Type { Handgun, Rifle, SMG, Shotgun, Melee, Sniper }
public enum Fire_Type { None, Single, Continuos }

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Weapon Stats", order = 1)]
public class Weapon_Stats : ScriptableObject {

    public Weapon_Type weaponType;
    public Fire_Type fireType;

    public string weaponName;
    public int magazineSize;
    public int ammoCarryingCapacity;
    public float fireRate;
    public float damageOutput;
    public float maxRange;
    public float maxDamageOutputRange;
    public float damageDropoff;
    public float reloadTime;
    public float aimFOV;

    [Range(0.1f, 2f)] public float baseRecoil = 1f;
    [Range(0.1f, 2f)] public float baseSpread = 1f;
    [Range(1, 10)]    public float weaponWeight;

    public GameObject metallicBulletHole;
    public Sprite weaponSprite;
    public GameObject weaponModel;

    public List<Vector3> recoilPattern = new List<Vector3>();
}
