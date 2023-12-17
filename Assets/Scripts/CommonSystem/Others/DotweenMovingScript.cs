using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class PRS {
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;

    public PRS(Vector3 _pos, Quaternion _rot, Vector3 _scale) {
        pos = _pos;
        rot = _rot;
        scale = _scale;
    }
}

public class DotweenMovingScript : MonoBehaviour {
    public void MoveTransform(PRS _prs, bool _relative = false) {
        if (_relative) {
            Vector3 originScale = transform.localScale;
            Vector3 newScale = new Vector3(originScale.x * _prs.scale.x, originScale.y * _prs.scale.y, originScale.z * _prs.scale.z);
            transform.position += _prs.pos;
            transform.rotation *= _prs.rot;
            transform.localScale = newScale;
        }
        else {
            transform.position = _prs.pos;
            transform.rotation = _prs.rot;
            transform.localScale = _prs.scale;
        }
    }

    public void MoveTransform(PRS _prs, float _timeDotween, Ease _ease, bool _relative = false) {
        if (_relative) {
            Vector3 originScale = transform.localScale;
            Vector3 newScale = new Vector3(originScale.x * _prs.scale.x, originScale.y * _prs.scale.y, originScale.z * _prs.scale.z);
            transform.DOMove(transform.position + _prs.pos, _timeDotween).SetEase(_ease);
            transform.DORotateQuaternion(transform.rotation * _prs.rot, _timeDotween).SetEase(_ease);
            transform.DOScale(newScale, _timeDotween).SetEase(_ease);
        }
        else {
            transform.DOMove(_prs.pos, _timeDotween).SetEase(_ease);
            transform.DORotateQuaternion(_prs.rot, _timeDotween).SetEase(_ease);
            transform.DOScale(_prs.scale, _timeDotween).SetEase(_ease);
        }
    }
}