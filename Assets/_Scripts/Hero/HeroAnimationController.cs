using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class HeroAnimationController : MonoBehaviour
{
    public event Action AttackAnimationEnded;

    private Animator _animator;
    private Dictionary<string, float> _animationLengths;
    private Vector3 _originalPosition;
    private Transform _parentObject;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _parentObject = GetComponentInParent<Hero>().transform;

        _animationLengths = new Dictionary<string, float>();
        var clips = _animator.runtimeAnimatorController.animationClips;

        foreach(AnimationClip clip in clips)
        {
            var paramName = clip.name.Replace("Animation", "");
            _animationLengths.Add(paramName, clip.length);
        }
    }

    private void Start()
    {
        _originalPosition = _parentObject.position;
    }

    private void OnAttackAnimationEnded()
    {
        AttackAnimationEnded?.Invoke();
    }

    public void Attack(Vector3 enemyPosition)
    {
        var paramName = "Attack";
        var length = _animationLengths[paramName] / 2;
        var speed = Vector3.Distance(_originalPosition, enemyPosition) / length;

        _animator.SetTrigger(paramName);

        StartCoroutine(MoveToEnemy(enemyPosition, speed));
    }

    public void Damaged()
    {
        _animator.SetTrigger("Damaged");
    }

    public void Dead()
    {
        _animator.SetTrigger("Dead");
    }

    private IEnumerator MoveToEnemy(Vector3 enemyPosition, float speed)
    {
        yield return StartCoroutine(MoveToPosition(enemyPosition, speed));
        StartCoroutine(MoveToPosition(_originalPosition, speed));
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float speed)
    {
        while (!_parentObject.position.AreEqual(targetPosition))
        {
            _parentObject.position = Vector3.MoveTowards(_parentObject.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }
}