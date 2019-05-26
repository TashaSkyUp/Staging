using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponSpawnProperties weaponSpawnProperties;

    public Weapon_Stats weaponStats;
    public ParticleSystem muzzleFlash;

    public LayerMask shootLayers;

    public bool randomRecoil = false;
    public int currentAmmoInClip = 0;
    public int totalAmmo = 0;
    public bool isShooting;
    public bool isReloading;

    public float recoilModifierADS = 0.5f;

    public float spreadModifierIdle = 0;
    public float spreadModifierWalking = 0.5f;
    public float spreadModifierSprinting = 1;
    public float spreadModifierJumping   = 1.5f;

    Animator m_Anim;
    Player_Input playerInput;
    Player_Controller playerController;
    CharacterController m_Player;
    Camera fpsCam;

    float fireRateDelay = 0;
    int recoilIndex = 0;
    float spreadModifier = 1;
    float recoilADS = 1;

    void Start()
    {
        fpsCam = transform.parent.GetComponent<Camera>();
        playerInput = transform.root.GetComponent<Player_Input>();
        playerController = transform.root.GetComponent<Player_Controller>();
        m_Player = transform.root.GetComponent<CharacterController>();
        m_Anim = GetComponent<Animator>();
        currentAmmoInClip = weaponStats.magazineSize;
        totalAmmo = weaponStats.ammoCarryingCapacity - weaponStats.magazineSize;
    }

    void Update()
    {
        if (!isShooting)
        {
            recoilIndex = 0;
        }

        Shoot();
        Reload();
    }

    #region Shoot
    void Shoot() {
        if (weaponStats.fireType == Fire_Type.Single) {
            SingleShotMechanics();
        }
        else if(weaponStats.fireType == Fire_Type.Continuos){
            ContinuosShotMechanics();
        }
    }
    #endregion

    #region Reload
    void Reload()
    {
        m_Anim.SetBool("Reloading", isReloading);
        if (playerInput.ReloadInput && currentAmmoInClip < weaponStats.magazineSize && totalAmmo > 0 && !isReloading) {
            isReloading = true;
            StartCoroutine(ReloadMechanics());
        }
    }
    #endregion

    #region Shoot Mechanics
    void SingleShotMechanics() {
        if (playerInput.ShootInput && Time.time > fireRateDelay && currentAmmoInClip > 0 && !isReloading)
        {
            fireRateDelay = Time.time + 1f / weaponStats.fireRate;
            m_Anim.SetBool("Shooting", true);

            currentAmmoInClip--;

            RaycastHit Hit;

            Vector3 originPos = CalculateSpread();

            if (Physics.Raycast(fpsCam.transform.position + originPos, fpsCam.transform.forward, out Hit, Mathf.Infinity , shootLayers))
            {
                GameObject bulletHole = Instantiate(weaponStats.metallicBulletHole, Hit.point + (Hit.normal * 0.001f), Quaternion.identity);
                bulletHole.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, Hit.normal);
                Destroy(bulletHole, 20f);
                Debug.Log(Hit.transform.name);
            }

            RecoilMechanics();
        }
    }

    void ContinuosShotMechanics() {
        isShooting = (playerInput.ShootContinuosInput && currentAmmoInClip > 0 && !isReloading) ? true : false;
        m_Anim.SetBool("Shooting", isShooting);
        if (isShooting && Time.time > fireRateDelay && currentAmmoInClip > 0)
        {
            fireRateDelay = Time.time + 1f / weaponStats.fireRate;
            currentAmmoInClip--;

            RaycastHit Hit;

            Vector3 originPos = CalculateSpread();

            if (Physics.Raycast(fpsCam.transform.position + originPos, fpsCam.transform.forward, out Hit, Mathf.Infinity, shootLayers))
            {
                GameObject bulletHole = Instantiate(weaponStats.metallicBulletHole, Hit.point + (Hit.normal * 0.001f), Quaternion.identity);
                bulletHole.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, Hit.normal);
                Destroy(bulletHole, 20f);
                Debug.Log(Hit.transform.name);
            }

            RecoilMechanics();
        }
    }
    #endregion

    #region Weapon Spread
    Vector3 CalculateSpread() {

        if (playerInput.transform.GetComponent<Weapon_Controller>().isAiming)
            recoilADS = recoilModifierADS;
        else
            recoilADS = 1;

        if (m_Player.isGrounded)
        {
            if (m_Player.velocity.magnitude > 0.5f && (playerController.isSprinting || playerController.isSliding))
                spreadModifier = spreadModifierSprinting;

            if (m_Player.velocity.magnitude > 0.5f && !playerController.isSprinting && !playerController.isSliding)
                spreadModifier = spreadModifierWalking;

            if (m_Player.velocity.magnitude < 0.5f)
                spreadModifier = spreadModifierIdle;
        }
        else
        {
            spreadModifier = spreadModifierJumping;
        }

        return (Random.insideUnitCircle * spreadModifier * weaponStats.baseSpread);
    }
    #endregion

    #region Reload Mechinaics
    IEnumerator ReloadMechanics() {
        yield return new WaitForSeconds(weaponStats.reloadTime);

        int ammoUsed = weaponStats.magazineSize - currentAmmoInClip;
        currentAmmoInClip = weaponStats.magazineSize;
        totalAmmo -= ammoUsed;

        if (totalAmmo < 0)
            totalAmmo = 0;

        m_Anim.SetBool("Reloading", false);
        isReloading = false;
    }
    #endregion

    #region Recoil Mechanics
    void RecoilMechanics() {
        Camera_Controller cameraController = transform.root.GetComponent<Camera_Controller>();
        Player_Controller playerController = cameraController.transform.GetComponent<Player_Controller>();

        Vector3 playerSpeed = new Vector3(playerController.GetComponent<CharacterController>().velocity.x, 0, playerController.GetComponent<CharacterController>().velocity.z);
        float speedMagnitude = playerSpeed.magnitude;

        if (randomRecoil)
            RandomRecoil(cameraController, playerController);
        else
            RecoilPattern(cameraController, playerController);
    }

    void RecoilPattern(Camera_Controller cameraController, Player_Controller playerController) {
        cameraController.xRotation -= weaponStats.recoilPattern[recoilIndex].x * weaponStats.baseRecoil * recoilADS;
        playerController.yRotation += weaponStats.recoilPattern[recoilIndex].y * weaponStats.baseRecoil * recoilADS;
        recoilIndex++;
    }

    void RandomRecoil(Camera_Controller cameraController, Player_Controller playerController) {
        int _recoilIndex = Random.Range(0, weaponStats.recoilPattern.Count - 1);
        cameraController.xRotation -= weaponStats.recoilPattern[_recoilIndex].x * weaponStats.baseRecoil * recoilADS;
        playerController.yRotation += weaponStats.recoilPattern[_recoilIndex].y * weaponStats.baseRecoil * recoilADS;
    }
    #endregion

    #region Animator Events
    public void PlayMuzzleFlash() {
        muzzleFlash.Play();
    }

    #endregion
}

#region Spawning
[System.Serializable]
public class WeaponSpawnProperties {
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;
}
#endregion
