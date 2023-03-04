using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>徘徊する蜘蛛</summary>
public class WanderingSpider : MonoBehaviour
{
    [SerializeField, Tooltip("目標との間隔、余裕を持った値にする")]
    float _dis = 5f;
    [SerializeField, Tooltip("Playerを察知出来る間隔、余裕を持った値にする")]
    float _playerPerceptionDis = 5f;
    [SerializeField, Tooltip("プレイヤーを追跡する時間")]
    float _trackingTime = 30f;
    float _countTime = 0f;
    [Tooltip("_pointsの要素数")]
    int _pointsNumber = 0;
    [Tooltip("0 == プレイヤー未発見、1 == プレイヤー発見")]
    int _mode;
    [Tooltip("playerを見つけたらTrue")]
    bool _playerPerception = false;
    GameObject _player = null;
    [SerializeField, Tooltip("徘徊する場所")]
    Vector3[] _points;
    NavMeshAgent _agent;
    MusicManager _musicM;
    Animator _anim = null;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameManager.Instance.Player;
        _musicM = GameManager.Instance.MusicManager;
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = _points[_pointsNumber];
        if (_player)
        {
            float distance = Vector3.Distance(transform.position, _player.transform.position);//自身とplayerの距離
            if(distance <= _playerPerceptionDis)//プレイヤーが近くにいたら
            {
                _playerPerception = true;
                if (_musicM.Bgm != BGM.playerPerception && _musicM.Bgm != BGM.PlayerFind)//他の蜘蛛がプレイヤーを察知していない、見つけていない場合
                {
                    _musicM.PlayBGM(BGM.playerPerception);//察知のBGMに切り替え
                }
                _mode = 1;
            }
            else
            {
                if (_playerPerception)//察知していたら
                {
                    _countTime += Time.deltaTime;
                    if (_countTime >= _trackingTime)
                    {
                        _countTime = 0;
                        _playerPerception = false;
                        _musicM.PlayBGM(BGM.Stage);
                        _mode = 0;
                    }
                }
                else
                {
                    _mode = 0;
                }
            }
            //if (distance > _playerFindDis)//playerを見つけられる距離より、距離が離れていたら
            //{
            //    if (_playerPerception)//playerが近くにいた（察知）場合、BGMが変更されているので
            //    {
            //        _countTime = 0;
            //        _musicM.PlayBGM(BGM.Stage);
            //        _playerPerception = false;
            //    }
            //    _mode = 0;
            //}
            //else
            //{
            //    _countTime += Time.deltaTime;
            //    if (_musicM.Bgm != MusicManager.BGM.playerPerception && _musicM.Bgm != MusicManager.BGM.PlayerFind)
            //    {
            //        _musicM.Bgm = MusicManager.BGM.playerPerception;
            //        _musicM.PlayBGM(_musicM.Bgm);
            //    }
            //    _mode = 1;
            //}
        }
        switch (_mode)
        {

            case 0:

                if (Vector3.Distance(transform.position, pos) < _dis)
                {
                    _pointsNumber++;
                    _pointsNumber = _pointsNumber % _points.Length;
                }
                _agent.SetDestination(pos);
                break;

            case 1:
                _agent.SetDestination(_player.transform.position);//プレイヤーに向かって進む
                break;
        }
    }

    private void LateUpdate()
    {
        if (_anim)
        {
            _anim.SetFloat("Speed", _agent.velocity.magnitude);
        }
    }
}
