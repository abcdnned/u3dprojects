﻿#pragma warning disable 649

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TY {
    // Author: Sergio Abreu García | https://sergioabreu.me

    public class ActiveRagdoll : MonoBehaviour {
        /// <summary> The unique ID of this Active Ragdoll instance. </summary>
        public uint ID { get; private set; }
        private static uint _ID_COUNT = 0;

        [Header("--- BODY ---")]
        [SerializeField] private Transform _animatedTorso;
        [SerializeField] private Rigidbody _physicalTorso;
        public Transform AnimatedTorso { get { return _animatedTorso; } }
        public Rigidbody PhysicalTorso { get { return _physicalTorso; } }


        public Transform[] AnimatedBones { get; private set; }
        public ConfigurableJoint[] Joints { get; private set; }

        /// <summary> Whether to constantly set the rotation of the Animated Body to the Physical Body's.</summary>
        public bool SyncTorsoPositions { get; set; } = true;
        public bool SyncTorsoRotations { get; set; } = true;

        private void Awake() {
            ID = _ID_COUNT++;
            if (AnimatedBones == null) AnimatedBones = _animatedTorso?.GetComponentsInChildren<Transform>();
            if (Joints == null) Joints = _physicalTorso?.GetComponentsInChildren<ConfigurableJoint>();
        }

    }
} // namespace ActiveRagdoll