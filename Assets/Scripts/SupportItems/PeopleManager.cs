using UnityEngine;

public class PeopleManager : MonoBehaviour
{
    public static PeopleManager Instance;
    private float peopleSpeed = 7f;

    private void Awake() => Instance = this;

    public void ModifyPeopleSpeed(float multiplier)
    {
        peopleSpeed *= multiplier;
    }

    public float GetPeopleSpeed() => peopleSpeed;
}
