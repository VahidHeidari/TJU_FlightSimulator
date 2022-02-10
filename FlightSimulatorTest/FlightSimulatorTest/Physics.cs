using System;
using Microsoft.DirectX;

namespace Framework
{
    struct PointMass
    {
        public float fMass;
        public Vector3 vDCoords;
        public Vector3 vCGCoords;
        public Vector3 vLocalInertia;
        public Vector3 vNormal;
        public float fIncidence;
        public float fDihedral;
        public float fArea;
        public int iFlap;
    }

    struct _PointMass
    {
        public float mass;
        public Vector3 designPosition;
        public Vector3 correctedPosition;
        public Vector3 localInertia;
    }

    struct RigidBody
    {
        public float fMass;
        public float fMassInverse;
        public Matrix mInertia;
        public Matrix mInertiaInverse;
        public Vector3 vPosition;
        public Vector3 vVelocity;
        public Vector3 vVelocityBody;
        public Vector3 vAngularVelocity;
        public Vector3 vEulerAngles;
        public float fSpeed;
        public Quaternion qOrientation;
        public Vector3 vForces;
        public Vector3 vMoments;
        public Vector3 vAcceleration;
    }

}
