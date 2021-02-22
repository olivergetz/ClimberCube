using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Camera camera;

    float screenTop;
    [SerializeField] float speed = 3f;
    [SerializeField] float deadZoneTop = 50f; //Top to bottom = negative to positive
    [SerializeField] float deadZoneBottom = 50f;

    float distance;

    [SerializeField] GameObject player;

    Vector3 playerPosition;
    Vector3 tempPosition = new Vector3(0,0,0);
    [SerializeField] Vector3 offset = new Vector3(0,0,0);

    IEnumerator coroutine;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        playerPosition = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = player.transform.position;

        tempPosition.y = playerPosition.y;

        distance = camera.transform.position.y - playerPosition.y;
        //Debug.Log(distance);

        //If Player Reaches edge of deadzone
        if (distance < -deadZoneTop)
        {
            //Debug.Log("Top Reached");
            //Move camera toward player
            coroutine = SmoothMovement(deadZoneTop);
            //StartCoroutine(coroutine);
            transform.position = Vector3.MoveTowards(transform.position, tempPosition + offset, speed * Time.deltaTime);
        }
        else if (distance > deadZoneBottom)
        {
            //Debug.Log("Bottom Reached");
            //Move camera toward player
            coroutine = SmoothMovement(deadZoneBottom);
            //StartCoroutine(coroutine);
            transform.position = Vector3.MoveTowards(transform.position, tempPosition + offset, speed * Time.deltaTime);
        }

    }

    IEnumerator SmoothMovement(float target)
    {
        //if (target < 0) target = target * -1;

        for (float f = 0.0f; distance > target; f = f + 0.2f)
        {
            Mathf.Lerp(camera.transform.position.y, playerPosition.y, f);
            yield return true;
        }

        //gameObject.transform.position = tempPosition + offset;

    }

    
    public IEnumerator CameraShake(float duration, float magnitude)
    {
        float originalSize = camera.orthographicSize;
        magnitude = magnitude + originalSize;
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            float t = Mathf.InverseLerp(0, duration, elapsedTime);
            camera.orthographicSize = Mathf.Lerp(magnitude, originalSize, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        camera.orthographicSize = originalSize;
    }
    

}
