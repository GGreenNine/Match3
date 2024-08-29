using System.Threading;
using DG.Tweening;
using QFSW.QC;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class PatrolMovementAnimation : MonoBehaviour
    {
        [SerializeField] private Transform _movingObject;
        [SerializeField] private Transform _startPos;
        [SerializeField] private Transform _endPos;
        [SerializeField] private float m_speed;

        private Sequence _sequence;
        
        [Command("StartPatrolling", MonoTargetType.All)]
        public void Animate()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            _movingObject.transform.position = _startPos.position;
            _sequence.Append(_movingObject.transform.DOMove(_endPos.position, m_speed));
            _sequence.Append(_movingObject.transform.DOMove(_startPos.position, m_speed));
            _sequence.SetLoops(int.MaxValue);
            _sequence.AppendCallback(() => { _sequence.Restart(); });
        }

        [Command("StopPatrolling", MonoTargetType.All)]

        public void Stop()
        {
            _sequence?.Kill();
            _sequence = null;
        }
    }
}