using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float moveSpeed;//How fast camera gets to desired position (determines camera lag)
    public float maxSize;//Max size of camera
    public float minSize;//Min size of camera
    public float changeSizeSpeed;//How fast camera adjusts

    private float currSize;//Curr size of camera
    private float desiredSize;

    public GameObject followTarget1;
    public GameObject followTarget2;

    private float diffInX;
    private float diffInY;
    private float dist;

    private float tar1X;
    private float tar1Y;
    private float tar2X;
    private float tar2Y;

    private float averageX;
    private float averageY;
    private Vector3 desiredPos;//Position that camera will go to


    private static bool cameraExists;

	// Use this for initialization
	void Start () {
        //Deals with any duplicates of the camera when any scene loading happens
        if (!cameraExists)
        {
            cameraExists = true;
            DontDestroyOnLoad(transform.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //The following code determines the camera's start position
        tar1X = followTarget1.transform.position.x;
        tar1Y = followTarget1.transform.position.y;
        tar2X = followTarget2.transform.position.x;
        tar2Y = followTarget2.transform.position.y;
        //Get the average of the x,y coordinates of both objects
        averageX = (tar1X+tar2X) / 2;
        averageY = (tar1Y+tar2Y) / 2;
        desiredPos = new Vector3(averageX, averageY, transform.position.z);//Camera's z-position stays the same
        transform.position = new Vector3(averageX, averageY, transform.position.z);
    }

    // Update is called once per frame
    void Update () {
        tar1X = followTarget1.transform.position.x;
        tar1Y = followTarget1.transform.position.y;
        tar2X = followTarget2.transform.position.x;
        tar2Y = followTarget2.transform.position.y;

        //Change camera size accordingly
        currSize = Camera.main.orthographicSize;
        diffInX = Mathf.Abs(tar2X - tar1X);
        diffInY = Mathf.Abs(tar2Y - tar1Y);
        dist = Mathf.Sqrt(Mathf.Pow(diffInX, 2) + Mathf.Pow(diffInY, 2));
        if(currSize > minSize && currSize < maxSize)//If diff is within the max and min size
        {
            //Change camera size
            if (dist < currSize)
            {
                if(currSize-changeSizeSpeed >= minSize)//If doing this doesn't make it go below minSize
                    Camera.main.orthographicSize -= changeSizeSpeed;//-= Time.deltaTime;
            }
            else if (dist > currSize)
            {
                if(currSize+changeSizeSpeed <= maxSize)//If doing this doesn't make it go above maxSize
                    Camera.main.orthographicSize += changeSizeSpeed;//+= Time.deltaTime;
            }
            //Same size- do nothing
        }


        //Get the average of the x,y coordinates of both objects
        averageX = (tar1X + tar2X) / 2;
        averageY = (tar1Y + tar2Y) / 2;
        desiredPos = new Vector3(averageX, averageY, transform.position.z);//Camera's z-position stays the same

        transform.position = Vector3.Lerp(transform.position, desiredPos, moveSpeed * Time.deltaTime);//Move camera
    }
}
