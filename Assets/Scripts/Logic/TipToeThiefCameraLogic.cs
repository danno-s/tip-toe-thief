using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Animator))]
public class TipToeThiefCameraLogic : TipToeThiefGuardLogic
{
    internal override void Start()
    {
        anm = GetComponent<Animator>();
        base.Start();
    }

    internal override void UpdateAnimation()
    {
        float angle = Vector2.SignedAngle(Vector2.right, lookingDirection);

        string clipName = anm.GetCurrentAnimatorClipInfo(0)[0].clip.name;   

        if (angle > -157.5 && angle < -112.5 && clipName != "LookLeftDown")
        {
            lookingDirection = new Vector3(-1, -1, 0);
            anm.Play("LookLeftDown");
        }
        else if (angle > -112.5 && angle < -67.5 && clipName != "LookDown")
        {
            lookingDirection = new Vector3(0, -1, 0);
            anm.Play("LookDown");
        }
        else if (angle > -67.5 && angle < -22.5 && clipName != "LookDownRight")
        {
            lookingDirection = new Vector3(1, -1, 0);
            anm.Play("LookDownRight");
        }
        else if (angle > -22.5 && angle < 22.5 && clipName != "LookRight")
        {
            lookingDirection = new Vector3(1, 0, 0);
            anm.Play("LookRight");
        }
        else if (angle > 22.5 && angle < 67.5 && clipName != "LookRightUp")
        {
            lookingDirection = new Vector3(1, 1, 0);
            anm.Play("LookRightUp");
        }
        else if (angle > 67.5 && angle < 112.5 && clipName != "LookUp")
        {
            lookingDirection = new Vector3(0, 1, 0);
            anm.Play("LookUp");
        }
        else if (angle > 112.5 && angle < 157.5 && clipName != "LookUpLeft")
        {
            lookingDirection = new Vector3(-1, 1, 0);
            anm.Play("LookUpLeft");
        }
        else if ((angle > 157.5 || angle < -157.5) && clipName != "LookLeft")
        {
            lookingDirection = new Vector3(-1, 0, 0);
            anm.Play("LookLeft");
        }
    }

    public override bool IsCat()
    {
        return false;
    }

    public override bool IsOwl()
    {
        return true;
    }

    internal override Vector3 GetSnappedDirection(Vector3 direction)
    {
        float angle = Vector2.SignedAngle(Vector2.right, lookingDirection);

        if (angle > -157.5 && angle < -112.5)
        {
            return new Vector3(-1, -1, 0);
        }
        else if (angle > -112.5 && angle < -67.5)
        {
            return new Vector3(0, -1, 0);
        }
        else if (angle > -67.5 && angle < -22.5)
        {
            return new Vector3(1, -1, 0);
        }
        else if (angle > -22.5 && angle < 22.5)
        {
            return new Vector3(1, 0, 0);
        }
        else if (angle > 22.5 && angle < 67.5)
        {
            return new Vector3(1, 1, 0);
        }
        else if (angle > 67.5 && angle < 112.5)
        {
            return new Vector3(0, 1, 0);
        }
        else if (angle > 112.5 && angle < 157.5)
        {
            return new Vector3(-1, 1, 0);
        }
        else
        {
            return new Vector3(-1, 0, 0);
        }
    }

    internal override IEnumerator LookingAtDistraction()
    {

        float angle = 180;

        while (angle != 0)
        {
            if (distractionTransform == null)
                break;

            RotateTowards(distractionTransform.position - transform.position, 4f, out angle);
            yield return null;
        }

        currentCoroutine = StartCoroutine("LookingBack");
    }

    internal override void OnDrawGizmos()
    {
#if UNITY_EDITOR
        switch (guardState)
        {
            case GuardState.Waiting:
                Handles.color = waitColor;
                break;
            case GuardState.RotatingTowards:
                Handles.color = rotateColor;
                break;
            case GuardState.Moving:
                Handles.color = moveColor;
                break;
            case GuardState.LookingTowards:
                Handles.color = lookColor;
                break;
        }
        Handles.DrawSolidArc(
          transform.position,
          transform.forward,
          Quaternion.Euler(0, 0, -fieldOfView / 2) * GetSnappedDirection(lookingDirection),
          fieldOfView,
          sightRange
        );
#endif
    }
}
