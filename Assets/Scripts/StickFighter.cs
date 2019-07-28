using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickFighter : MonoBehaviour
{
    public int player;
    private float disableParticleSystem;
    private int health = 100;
    public RectTransform foregroundPanel;
    GameObject stickFighter;
    Transform leg1Parent;
    Transform leg2Parent;
    ParticleSystem.EmissionModule emissionModule;

    // Start is called before the first frame update
    void Start()
    {
        stickFighter = gameObject;
        leg1Parent = stickFighter.transform.Find("Leg1Parent");
        leg2Parent = stickFighter.transform.Find("Leg2Parent");
        emissionModule = stickFighter.transform.Find("PsJetpack").GetComponent<ParticleSystem>().emission;
    }

    // Update is called once per frame
    void Update()
    {
        // Adjust the angle of the legs (Does not really adds to the game but whatever)
        leg1Parent.rotation = Quaternion.Euler(new Vector3(0, 0, stickFighter.transform.rotation.eulerAngles.z + ((Input.GetAxis("Joy" + player + "Axis1") - 0.5f) * 100)));
        leg2Parent.rotation = Quaternion.Euler(new Vector3(0, 0, stickFighter.transform.rotation.eulerAngles.z - ((Input.GetAxis("Joy" + player + "Axis2") - 0.5f) * 100)));

        // Rotate the entire prefab using the left analogue stick
        stickFighter.transform.Rotate(0, 0, (Input.GetAxis("Joy" + player + "Axis3") * 1.7f));

        // Swing the sword using the xbox one controller's A button
        if (Input.GetKeyDown("joystick " + player + " button 0"))
        {
            stickFighter.GetComponent<Animator>().Play("Hit");
        }

        // Flip the stick figther along his X axis using the xbox one controller's B button
        if (Input.GetKeyDown("joystick " + player + " button 1"))
        {
            stickFighter.transform.localScale = new Vector3(stickFighter.transform.localScale.x * -1, stickFighter.transform.localScale.y, stickFighter.transform.localScale.z);
        }

        // Give the stickfigter a little jetpack like push to make flying moves possible
        if (Input.GetKeyDown("joystick " + player + " button 2"))
        {
            stickFighter.GetComponent<Rigidbody2D>().AddForce(stickFighter.transform.up * 150);
            
            // Enable the jetpack's emission
            emissionModule.enabled = true;

            // Set a time after which the emission should be disabled again (Now + 0.3s)
            disableParticleSystem = Time.time + .3f;
        }

        // Pressing the xbox one controller's menue button starts a new roung
        if (Input.GetKeyDown("joystick " + player + " button 7"))
        {
            Main.instance.Replay();
        }

        // Pressing the xbox one controller's back button quits the game
        if (Input.GetKeyDown("joystick " + player + " button 6"))
        {
            Main.instance.Quit();
        }

        // Check if the jetpack exhaust should be disabled again
        if (Time.time > disableParticleSystem)
        {
            emissionModule.enabled = false;
        }

        // Check if this fighter is death
        if (health <= 0)
        {
            gameObject.SetActive(false);
            Main.instance.FinishRound(player);
        }
    }

    // Here, we calculate the recieved damage
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only take damage if sword was colliding wiht me, not legs or arms or so
        // Also Walls are not that painfull
        if (!collision.collider.name.Equals("Sword") || collision.collider.name.StartsWith("Wall"))
        {
            return;
        }

        // Log collisions to understand what damage has been done
        Debug.Log("Player " + player + " got hit on " + collision.otherCollider.name + " from " + collision.collider.name);

        // Diffent damage for different body parts
        switch (collision.otherCollider.name)
        {
            case "Arm":
                health -= 4;
                break;
            case "Leg":
                health -= 2;
                break;
            case "Head":
                health -= 10;
                break;
            case "body":
                health -= 7;
                break;
        }

        // Set the health bar to the correct length
        // healthbar size 200px, max health is 1oo -> therefore healthbar size = health * 2
        foregroundPanel.sizeDelta = new Vector2(health * 2, 25);
    }
}