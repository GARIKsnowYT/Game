using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public enum WeaponType
    {
        None,
        Pickaxe,
        AutomaticRifle,
        Sword
    }

    // Ссылки на игровые объекты для каждого типа оружия
    public GameObject[] weapons = new GameObject[9];
    public HandShake handShakeScript;

    private WeaponType currentWeapon = WeaponType.None;

    void Start()
    {
        HideAllWeapons();
        handShakeScript = GetComponent<HandShake>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchWeapon(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchWeapon(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SwitchWeapon(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SwitchWeapon(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SwitchWeapon(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SwitchWeapon(8);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SwitchWeapon(-1); // -1 для отключения оружия
        }
    }

    // Метод для скрытия всех оружий
    void HideAllWeapons()
    {
        foreach (var weapon in weapons)
        {
            if (weapon != null)
                weapon.SetActive(false);
        }
    }

    // Метод для отображения определенного оружия
    void ShowWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weapons.Length)
        {
            var weapon = weapons[weaponIndex];
            bool isCurrentWeapon = currentWeapon == (WeaponType)weaponIndex;
            if (weapon != null)
            {
                if (!isCurrentWeapon)
                {
                    HideAllWeapons();
                    weapon.SetActive(true);
                    currentWeapon = (WeaponType)weaponIndex;
                }
                else
                {
                    weapon.SetActive(false);
                    currentWeapon = WeaponType.None;
                }
            }
        }
    }

    // Метод для переключения на следующее свободное оружие
    void SwitchWeapon(int startFromIndex)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            int index = (i + startFromIndex + weapons.Length) % weapons.Length;
            if (weapons[index] != null)
            {
                ShowWeapon(index);
                break;
            }
        }
    }
}
