using System;
using UnityEditor;
using UnityEngine;

namespace TY {
    // Author: Sergio Abreu García | https://sergioabreu.me

    public class AnimationModule : Module {
        [Header("--- BODY ---")]
        /// <summary> Required to set the target rotations of the joints </summary>
        private Quaternion[] _initialJointsRotation;
        private ConfigurableJoint[] _joints;
        private Transform[] _animatedBones;

        private void Start() {
            _joints = _activeRagdoll.Joints;
            _animatedBones = _activeRagdoll.AnimatedBones;
            _initialJointsRotation = new Quaternion[_joints.Length];
            for (int i = 0; i < _joints.Length; i++) {
                _initialJointsRotation[i] = _joints[i].transform.localRotation;
                // Debug.Log(this.GetType().Name + " _joints[i] " + i + " " + _joints[i]);
            }
            for (int i = 0; i < _animatedBones.Length; ++i) {
                // Debug.Log(this.GetType().Name + " bone " + i + " " + _animatedBones[i]);
            }
        }

        void FixedUpdate() {
            UpdateJointTargets();
        }


        /// <summary> Makes the physical bones match the rotation of the animated ones </summary>
        private void UpdateJointTargets() {
            for (int i = 0; i < _joints.Length; i++) {
                // Debug.Log(this.GetType().Name + "update _joints" + i);
                ConfigurableJointExtensions.SetTargetRotationLocal(_joints[i], _animatedBones[i + 1].localRotation, _initialJointsRotation[i]);
            }
        }
    }
} // namespace ActiveRagdoll