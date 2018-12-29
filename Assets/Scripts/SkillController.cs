﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillController : MonoBehaviour
{

    private float speed = 12f;

    private Vector3 startPos;

    private static GameObject rina;

    private static GameObject[] spell = new GameObject[2];

    public static bool[] activateSpellBool = { false, false };
    static bool continueBool = false;
    static bool right = true;
    IEnumerator enumerator = null;

    // Use this for initialization
    void Start()
    {
        defaultSkills();
    }

    void defaultSkills()
    {
        Debug.Log("Selecting default spell");
        selectSpell(0, "FlameRing");
        selectSpell(1, "Halo");
    }

    public void selectSpell(int a, string spellString)
    {
        Debug.Log("Selecting spell: " + spellString);
        rina = GameObject.Find("Rina");
        spell[a] = Instantiate(Resources.Load(spellString)) as GameObject;
        spell[a].transform.SetParent(rina.transform);
        spell[a].GetComponentInChildren<ParticleSystemRenderer>().enabled = false;
        spell[a].name = spell[a].name.Replace("(Clone)", "");
        spell[a].transform.position = new Vector3(rina.transform.position.x+1, rina.transform.position.y, rina.transform.position.z);
    }

    public void resetSpell(string spellString)
    {
        GameObject[] button = GameObject.FindGameObjectsWithTag(spellString);
        button[0].GetComponent<Button>().interactable = true;
        Debug.Log("Reseting spell: " + spellString);
        for (int a = 0; a < 2; a++)
        {
            if (spell[a].name == spellString)
            {
                spell[a].transform.SetParent(rina.transform);
                activateSpellBool[a] = false;
                if (enumerator != null)
                    StopCoroutine(enumerator);
                StartCoroutine(resetSpellCoroutine(a));
                break;
            }
        }
    }

    public void activateSpell(string spellString)
    {
        GameObject[] button = GameObject.FindGameObjectsWithTag(spellString);
        button[0].GetComponent<Button>().interactable = false;
        Debug.Log("Activating spell");
        for (int a = 0; a < 2; a++)
        {
            if (spell[a].name == spellString)
            {
                right = CharacterController2D.m_FacingRight;
                if (right)
                    spell[a].transform.position = new Vector3(rina.transform.position.x+1, rina.transform.position.y, rina.transform.position.z);
                else
                    spell[a].transform.position = new Vector3(rina.transform.position.x-1, rina.transform.position.y, rina.transform.position.z);
                spell[a].transform.parent = null;
                activateSpellBool[a] = true;
                enumerator = resetSpellCoroutineWithTime(spellString);
                StartCoroutine(enumerator);
                StartCoroutine(activateSpellCoroutine(a));
            }
        }
    }

    IEnumerator activateSpellCoroutine(int a)
    {
        while (activateSpellBool[a])
        {
            yield return null;
            spell[a].GetComponentInChildren<ParticleSystemRenderer>().enabled = true;
            if (right)
                spell[a].transform.Translate(speed * Time.deltaTime, 0, 0);
            else
                spell[a].transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
        continueBool = true;
        yield return 0;
    }

    IEnumerator resetSpellCoroutine(int a)
    {
        yield return new WaitUntil(() => continueBool == true);
        spell[a].GetComponentInChildren<ParticleSystemRenderer>().enabled = false;
        spell[a].transform.position = new Vector3(rina.transform.position.x+1, rina.transform.position.y, rina.transform.position.z);
    }

    IEnumerator resetSpellCoroutineWithTime(string spellString)
    {
        yield return new WaitForSeconds(3);
        resetSpell(spellString);
    }
    
}
