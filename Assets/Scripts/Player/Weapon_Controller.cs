using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Controller : MonoBehaviour
{
    Player_Input playerInput;
    CharacterController m_Player;
    //NOTE : Primary Weapon - index at 0, secondary weapon - index at 1, tactical weapons - index 2 and 3 //

    [SerializeField] GameObject       weaponHolder;
    [SerializeField] List<GameObject> startingWeapons   = new List<GameObject>();
    [SerializeField] List<Weapon>     availableWeapons  = new List<Weapon>();
    [SerializeField] int              activeWeaponIndex = 0;
    [SerializeField] float            m_FOVLerpAmount = 20;

    [HideInInspector] public Weapon activeWeapon;
    [HideInInspector] public bool   isAiming;

    float originalFOV;

    public Animator ActiveWeaponAnimator
    { get
        {   if (availableWeapons.Count > 0)
                return availableWeapons[activeWeaponIndex].GetComponent<Animator>();
            else
                return null;
        }
    }

    private void Start()
    {
        playerInput = GetComponent<Player_Input>();
        m_Player = GetComponent<CharacterController>();
        originalFOV = Camera.main.fieldOfView;
        SpawnWeapon();
        InitWeapon();
    }

    private void Update()
    {
        ChangeWeapon();
        ADS();
    }

    void ChangeWeapon() {
        if (playerInput.MouseScroll > 0) {
            activeWeapon.isShooting = activeWeapon.isReloading = false;
            if (activeWeaponIndex < availableWeapons.Count - 1)
                activeWeaponIndex++;
            else
                activeWeaponIndex = 0;

            InitWeapon();
        }
        else if (playerInput.MouseScroll < 0) {
            activeWeapon.isShooting = activeWeapon.isReloading = false;
            if (activeWeaponIndex > 0)
                activeWeaponIndex--;
            else
                activeWeaponIndex = availableWeapons.Count - 1;

            InitWeapon();
        }
    }

    void SpawnWeapon() {
        for (int i = 0; i < startingWeapons.Count; i++)
        {
            GameObject m_Weapon = Instantiate(startingWeapons[i], weaponHolder.transform);
            m_Weapon.transform.localPosition = startingWeapons[i].GetComponent<Weapon>().weaponSpawnProperties.spawnPosition;
            m_Weapon.transform.localRotation = Quaternion.Euler(startingWeapons[i].GetComponent<Weapon>().weaponSpawnProperties.spawnRotation);
            m_Weapon.name = m_Weapon.GetComponent<Weapon>().weaponStats.weaponName;
            availableWeapons.Add(m_Weapon.GetComponent<Weapon>());
        }
    }

    void InitWeapon() {
        for (int i = 0; i < availableWeapons.Count; i++)
        {
            if (i == activeWeaponIndex)
                availableWeapons[i].gameObject.SetActive(true);
            else
                availableWeapons[i].gameObject.SetActive(false);
        }
        activeWeapon = availableWeapons[activeWeaponIndex].GetComponent<Weapon>();
    }

    void ADS() {
        isAiming = playerInput.AimInput && !activeWeapon.isReloading;
        if (isAiming && m_Player.isGrounded)
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, activeWeapon.weaponStats.aimFOV, m_FOVLerpAmount * Time.deltaTime);
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView,originalFOV, m_FOVLerpAmount * Time.deltaTime);
        }
        ActiveWeaponAnimator.SetBool("Aiming", isAiming);
    }
}
