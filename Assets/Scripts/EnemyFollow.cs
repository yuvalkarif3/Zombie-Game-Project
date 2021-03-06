﻿using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (Seeker))]
public class EnemyFollow : MonoBehaviour { // A class which uses the A* Pathfinder to follow the enemy around the map 

	// What to chase?
	public Transform target;

	// How many times each second we will update our path
	public float updateRate = 2f;

	// Caching
	private Seeker seeker;
	private Rigidbody2D rb;

	//The calculated path
	public Path path;
	public float distance = 0;

	//The AI's speed per second
	public float speed = 300f;
	public ForceMode2D fMode;

	[HideInInspector]
	public bool pathIsEnded = false;

	// The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 3;

	private int currentWaypoint = 0;

	void Start () {
		seeker = GetComponent<Seeker>();
		rb = GetComponent<Rigidbody2D>();
		target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>(); 

		if (target == null) {
			Debug.LogError ("No Player found? PANIC!");
			return;
		}

		// Start a new path to the target position, return the result to the OnPathComplete method
		seeker.StartPath (transform.position, target.position, OnPathComplete);

		StartCoroutine (UpdatePath ());
	}

	IEnumerator UpdatePath () {
		if (target == null) {
			//TODO: Insert a player search here.
			yield return false;
		}

		// Start a new path to the target position, return the result to the OnPathComplete method
		seeker.StartPath (transform.position, target.position, OnPathComplete);

		yield return new WaitForSeconds ( 1f/updateRate );
		StartCoroutine (UpdatePath());
	}

	public void OnPathComplete (Path p) {
		Debug.Log ("We got a path. Did it have an error? " + p.error);
		if (!p.error) {
			path = p;
			currentWaypoint = 0;
		}
	}

	void FixedUpdate () {
		target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>(); 
		if (target == null) {
			//TODO: Insert a player search here.
			return;
		}

		

		if (path == null)
			return;

		if (currentWaypoint >= path.vectorPath.Count) {
			if (pathIsEnded)
				return;

			Debug.Log ("End of path reached.");
			pathIsEnded = true;
			return;
		}
		pathIsEnded = false;

		//Direction to the next waypoint
		Vector3 dir = ( path.vectorPath[currentWaypoint] );//- transform.position );//.normalized;
		//dir *= speed * Time.fixedDeltaTime;

		//Move the AI
		//rb.AddForce (dir, fMode);
		if (target != null) {
			if (Vector2.Distance (transform.position, target.position) > distance) {
				transform.position = Vector2.MoveTowards (transform.position, dir, speed * Time.fixedDeltaTime);
			}
		}

		float dist = Vector3.Distance (transform.position, path.vectorPath[currentWaypoint]);
		if (dist < nextWaypointDistance) {
			currentWaypoint++;
			return;
		}
	}

}
