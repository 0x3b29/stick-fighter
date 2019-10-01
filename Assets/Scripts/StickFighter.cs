using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickFighter : MonoBehaviour
{
    public int player;

    private float disableParticleSystem;
    private int health = 100;
    private const float legSlurp = 0.075f;
    private bool resetLeftLegWithoutSlerp = false;
    private bool resetRightLegWithoutSlerp = false;

    public RectTransform foregroundPanel;

    // Stick fighter
    private GameObject stickFighter;
    private Transform leg1Parent;
    private Transform leg2Parent;
    private ParticleSystem.EmissionModule emissionModule;

    // Keyboard inputs
    public KeyCode rotateLeft;
    public KeyCode rotateRight;
    public KeyCode jump;
    public KeyCode hit;
    public KeyCode flip;
    public KeyCode leftLeg;
    public KeyCode rightLeg;

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

        // Set the left leg
        float leftLegControllerInput = Input.GetAxis("Joy" + player + "Axis1");
        if (leftLegControllerInput != 0 || resetLeftLegWithoutSlerp)
        {
            // The controller is beeing used
            leg1Parent.rotation = Quaternion.Euler(new Vector3(0, 0, stickFighter.transform.rotation.eulerAngles.z + (leftLegControllerInput - 0.5f) * 100));
            
            // If controller axis was beeing used, this flag must be set to avoid slerp. 
            if (leftLegControllerInput != 0)
            {
                resetLeftLegWithoutSlerp = true;
            }  
            else
            {
                resetLeftLegWithoutSlerp = false;
            }
        }
        else
        {
            // The controller is not beeing used (or in neutral)
            if (Input.GetKey(leftLeg))
            {
                Quaternion stop = Quaternion.Euler(new Vector3(0, 0, stickFighter.transform.rotation.eulerAngles.z + (1f - 0.5f) * 100));
                leg1Parent.rotation = Quaternion.Slerp(leg1Parent.rotation, stop, legSlurp);
            }
            else
            {
                Quaternion stop = Quaternion.Euler(new Vector3(0, 0, stickFighter.transform.rotation.eulerAngles.z + (0 - 0.5f) * 100));
                leg1Parent.rotation = Quaternion.Slerp(leg1Parent.rotation, stop, legSlurp);
            }
        }

        // Set the right leg
        float rightLegControllerInput = Input.GetAxis("Joy" + player + "Axis2");
        if (rightLegControllerInput != 0 || resetRightLegWithoutSlerp)
        {
            // The controller is beeing used
            leg2Parent.rotation = Quaternion.Euler(new Vector3(0, 0, stickFighter.transform.rotation.eulerAngles.z - (rightLegControllerInput - 0.5f) * 100));

            // If controller axis was beeing used, this flag must be set to avoid slerp. 
            if (rightLegControllerInput != 0)
            {
                resetRightLegWithoutSlerp = true;
            }
            else
            {
                resetRightLegWithoutSlerp = false;
            }
        }
        else
        {
            // The controller is not beeing used (or in neutral)
            if (Input.GetKey(rightLeg))
            {
                Quaternion stop = Quaternion.Euler(new Vector3(0, 0, stickFighter.transform.rotation.eulerAngles.z - (1f - 0.5f) * 100));
                leg2Parent.rotation = Quaternion.Slerp(leg2Parent.rotation, stop, legSlurp);
            }
            else
            {
                Quaternion stop = Quaternion.Euler(new Vector3(0, 0, stickFighter.transform.rotation.eulerAngles.z - (0 - 0.5f) * 100));
                leg2Parent.rotation = Quaternion.Slerp(leg2Parent.rotation, stop, legSlurp);
            }
        }

        // Rotate the fighter

        // Using the left analogue stick
        stickFighter.transform.Rotate(0, 0, (Input.GetAxis("Joy" + player + "Axis3") * 1.7f));

        // Using the rotate keys
        if (Input.GetKey(rotateLeft))
        {
            stickFighter.transform.Rotate(0, 0, (-1.7f));
        }

        if (Input.GetKey(rotateRight))
        {
            stickFighter.transform.Rotate(0, 0, (1.7f));
        }

        // Swing the sword 
        // using the xbox one controller's A button OR the keyboard's hit key  
        if (Input.GetKeyDown("joystick " + player + " button 0") || Input.GetKeyDown(hit))
        {
            stickFighter.GetComponent<Animator>().Play("Hit");
        }

        // Flip the stick figther along his X axis 
        // using the xbox one controller's B button OR the keyboard's flip key
        if (Input.GetKeyDown("joystick " + player + " button 1") || Input.GetKeyDown(flip))
        {
            stickFighter.transform.localScale = new Vector3(stickFighter.transform.localScale.x * -1, stickFighter.transform.localScale.y, stickFighter.transform.localScale.z);
        }

        // Give the stickfigter a little jetpack like push to make flying moves possible
        // using the xbox one controller's X button OR the keyboard's jump key
        if (Input.GetKeyDown("joystick " + player + " button 2") || Input.GetKeyDown(jump))
        {
            stickFighter.GetComponent<Rigidbody2D>().AddForce(stickFighter.transform.up * 150);
            
            // Enable the jetpack's emission
            emissionModule.enabled = true;

            // Set a time after which the emission should be disabled again (Now + 0.3s)
            disableParticleSystem = Time.time + .3f;
        }

        // Start a new roung
        // pressing the xbox one controller's menue button OR the space bar
        if (Input.GetKeyDown("joystick " + player + " button 7") || Input.GetKeyDown(KeyCode.Space))
        {
            Main.instance.Replay();
        }

        // Quits the game
        // pressing the xbox one controller's back button OR the escape key
        if (Input.GetKeyDown("joystick " + player + " button 6") || Input.GetKeyDown(KeyCode.Escape))
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