using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDragCamera : MonoBehaviour
{
	private static readonly float PanSpeed = 10f;
	private static readonly float ZoomSpeedTouch = 0.01f;
	private static readonly float ZoomSpeedMouse = 10f;
	
	private static readonly float[] BoundsX = new float[]{-12f, 12f};
	private static readonly float[] BoundsZ = new float[]{-12f, 12f};
	private static readonly float[] ZoomBounds = new float[]{-4f, 4f};

	public GameObject cam;

	private Vector3 lastPanPosition;
	private int panFingerId; // Touch mode only

	private bool wasZoomingLastFrame; // Touch mode only
	private Vector2[] lastZoomPositions; // Touch mode only

	public float mouseSensitivity;
	private Vector3 lastPosition;

	public Transform basePos;
	public bool gameEnded;

	private Vector3 targetPosition;
	private float transitionSpeed = 0.01f;
	private float fraction = 0f;

	private float zoomLevel = 0f;
	private Animator c_anim;
	// Start is called before the first frame update
	void Start()
	{
		gameEnded = false;
		c_anim = GetComponent<Animator>();

		c_anim.enabled = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (gameEnded) {
				
			if(fraction < 1)
				fraction += Time.deltaTime * transitionSpeed;

			transform.position = Vector3.Lerp(transform.position, targetPosition, fraction);
			return;
		}
		
		if (Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer) {
			HandleTouch();
		} else {
			HandleMouse();
		}
	}
	private float lastClickTime;

	void HandleTouch() {
		switch(Input.touchCount) {

		case 1: // Panning
			wasZoomingLastFrame = false;
			
			if (EventSystem.current.IsPointerOverGameObject(0)) 
				return;

			// If the touch began, capture its position and its finger ID.
			// Otherwise, if the finger ID of the touch doesn't match, skip it.
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began) {
				lastPanPosition = touch.position;
				panFingerId = touch.fingerId;
				lastClickTime = Time.time;
			} else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved) {
				PanCamera(touch.position);
			}else if (touch.phase == TouchPhase.Ended && Time.time - lastClickTime <= 0.2f) {
				GameObject.Find("GameLogic").GetComponent<GameLogic>().HandleTouch(0);
			}

			break;

		case 2: // Zooming
			Vector2[] newPositions = new Vector2[]{Input.GetTouch(0).position, Input.GetTouch(1).position};
			if (!wasZoomingLastFrame) {
				lastZoomPositions = newPositions;
				wasZoomingLastFrame = true;
			} else {
				// Zoom based on the distance between the new positions compared to the 
				// distance between the previous positions.
				float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
				float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
				float offset = newDistance - oldDistance;

				ZoomCamera(offset, ZoomSpeedTouch);

				lastZoomPositions = newPositions;
			}
			break;
			
		default: 
			wasZoomingLastFrame = false;
			break;
		}
	}

	void HandleMouse() {
		// On mouse down, capture it's position.
		// Otherwise, if the mouse is still down, pan the camera.

		if (EventSystem.current.IsPointerOverGameObject()) 
			return;
		
		if (Input.GetMouseButtonDown(0)) {
			lastPanPosition = Input.mousePosition;
			lastClickTime = Time.time;

		} else if (Input.GetMouseButton(0)) {
			PanCamera(Input.mousePosition);
		}

		if (Input.GetMouseButtonUp(0)) 
		{
			if(Time.time - lastClickTime <= 0.2f)
				GameObject.Find("GameLogic").GetComponent<GameLogic>().HandleMouse();
		}

		// Check for scrolling to zoom the camera
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		ZoomCamera(scroll, ZoomSpeedMouse);
	}

	public void SetGameOverCamera(){
		targetPosition = basePos.position;
		transform.eulerAngles = new Vector3(0f, 0f, 0f);

		c_anim.enabled = true;
		GetComponent<Animator>().SetTrigger("GameOverTrigger");

		gameEnded = true;
	}

	void PanCamera(Vector3 newPanPosition) {
		// Determine how much to move the camera
		Vector3 offset = cam.GetComponent<Camera>().ScreenToViewportPoint(lastPanPosition - newPanPosition);
		Vector3 move = new Vector3(offset.x * PanSpeed, 0, offset.y * PanSpeed);
		
		// Perform the movement
		transform.Translate(move, Space.World);  
		
		// Ensure the camera remains within bounds.
		Vector3 pos = transform.position;
		pos.x = Mathf.Clamp(transform.position.x, BoundsX[0], BoundsX[1]);
		pos.z = Mathf.Clamp(transform.position.z, BoundsZ[0], BoundsZ[1]);
		transform.position = pos;

		// Cache the position
		lastPanPosition = newPanPosition;
	}
	

	void ZoomCamera(float offset, float speed) {
		if (offset == 0) {
			return;
		}

		zoomLevel += offset * speed;
		zoomLevel = Mathf.Clamp(zoomLevel, ZoomBounds[0], ZoomBounds[1]);

		Vector3 pos = cam.transform.localPosition;
		pos.z = zoomLevel;
		cam.transform.localPosition = pos;

		//cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - (offset * speed), ZoomBounds[0], ZoomBounds[1]);

		//cam.transform.position.z = Mathf.Clamp(cam.transform.position.z - (offset * speed), ZoomBounds[0], ZoomBounds[1]);

		// Ensure the camera remains within bounds.
		

	}
}