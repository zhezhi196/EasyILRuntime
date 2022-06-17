using System;
using Module;
using UnityEngine;
using UnityEngine.AI;

public class Confrontation : SimpleBehavior
{
    private int _moveDir;
    public float standTime = 0;
    public AttackMonster monster;
    public int switchCount;
    public IConfrontationSkill dodge;
    public SkillCtrl skillCtrl;

    private Vector3 smothFoward;

    public bool isBack;

    public int moveDir
    {
        get
        {
            if (dodge.station == SkillStation.OnRelease) return dodge.moveToward;
            if (isBack) return 0;
            return _moveDir;
        }
    }

    public Vector3 faceDirection
    {
        get { return Player.player.chasePoint - monster.transform.position; }
    }

    public static int GetMoveDir(bool isBack,float distance, NavMeshAgent agent)
    {
        NavMeshHit leftHit;
        NavMeshHit rightHit;
        bool left = agent.Raycast(-agent.transform.right * distance, out leftHit);
        bool right = agent.Raycast(agent.transform.right * distance, out rightHit);
        int result = 0;
        if (left && right)
        {
            result = -(leftHit.distance.CompareTo(rightHit.distance));

        }
        else if (left && !right)
        {
            result = -1;
        }
        else if (!left && right)
        {
            result = 1;
        }
        else
        {
            result = RandomHelper.RandomSymbol();
        }

        if (isBack)
        {
            return -result;
        }
        else
        {
            return result;
        }
    }

    public override void OnStart(ISimpleBehaviorObject owner, object[] args)
    {
        base.OnStart(owner, args);
        isEnd = false;
        monster = (AttackMonster) owner;
        standTime = 0;
        _moveDir = GetMoveDir(isBack, 5, monster.navmesh);
        switchCount = 0;
        if (this.dodge == null)
        {
            skillCtrl = owner.GetAgentCtrl<SkillCtrl>();
            for (int i = 0; i < skillCtrl.allSkill.Count; i++)
            {
                if (skillCtrl.allSkill[i] is IConfrontationSkill dodge)
                {
                    this.dodge = dodge;
                    this.dodge.behavior = this;
                }
            }
        }

        isBack = true;
    }

    public override TaskStatus OnUpdate()
    {
        var standPostion = GetMinWallPoint(this.moveDir * monster.transform.right);
        CaclutateMove(standPostion);
        if (!isBack)
        {
            var moveDir = standPostion - monster.transform.position;
            var faceDir = Player.player.chasePoint - monster.transform.position;
            Quaternion rotation = Quaternion.LookRotation(new Vector3(faceDir.x, 0, faceDir.z));
            Vector3 dir = (Quaternion.Inverse(rotation) * moveDir).normalized;
            dir = new Vector3(-dir.x, 0, dir.z);
            SmoothDampToDir(dir);
        }
        else
        {
            SmoothDampToDir(new Vector3(0, 0, 1));
        }

        if (this.dodge.station == SkillStation.Ready && this.dodge.isWanted)
        {
            skillCtrl.TryRelease(dodge, null);
        }

        return TaskStatus.Running;
    }

    public Vector3 GetMovePoint(Vector3 dir)
    {
        if (dodge != null && dodge.station == SkillStation.OnRelease)
        {
            return GetMinWallPoint(dodge.moveToward * monster.transform.right);
        }
        else
        {
            if (monster.toPlayerDistance <= monster.currentLevel.dbData.standMin - 0.4f)
            {
                //往后走
                return GetMinWallPoint(Vector3.zero);
            }
            else
            {
                //往左或者往右走
                return GetMinWallPoint(dir);
            }
        }
    }

    public void CaclutateMove(Vector3 standPostion)
    {
        standTime += monster.GetDelatime(false);
        if (standTime >= monster.currentLevel.dbData.standTime)
        {
            Exit();
        }
        else
        {
            if (monster.missPlayerTime >= 1)
            {
                SwitchFightLevel();
            }

            monster.MoveTo(MoveStyle.Walk, standPostion, (status, complete) =>
            {
                if (complete)
                {
                    SwitchFightLevel();
                    isBack = false;
                }
            });
        }
    }

    public void Exit()
    {
        monster.ExitConfrontation();
    }

    public async void SwitchFightLevel()
    {
        if (moveDir == 1)
        {
            _moveDir = -1;
        }
        else if (moveDir == -1)
        {
            _moveDir = 1;
        }

        switchCount++;
        if (switchCount >= 4)
        {
            Exit();
        }

        Async.StopAsync(this);
        await Async.WaitforSeconds(2, this);
        switchCount = 0;
    }


    public Vector3 GetMinWallPoint(Vector3 dir)
    {
        NavMeshHit hit;
        Vector3 minPoint = GetOriPoint(monster.transform.position, Player.player.chasePoint);
        Vector3 tar = minPoint + dir.normalized;
        if (monster.navmesh.Raycast(tar, out hit))
        {
            return hit.position;
        }
        else
        {
            return tar;
        }
    }

    private Vector3 GetOriPoint(Vector3 monsterPoint, Vector3 playerPoint)
    {
        float distance = monsterPoint.Distance(playerPoint);
        Vector3 minPoint = (monsterPoint - playerPoint).normalized * Mathf.Clamp(distance, monster.currentLevel.dbData.standMin, monster.currentLevel.dbData.standMax) + playerPoint;
        return minPoint;
    }

    public override void OnDrawGizmos()
    {
        //Gizmos.DrawIcon(standPostion + Vector3.up, "duizhi");
    }

    public bool isEnd;

    public override void OnEnd()
    {
        switchCount = 0;
        monster.animator.SetFloat("moveFoward", -1);
        monster.animator.SetFloat("moveLeft", 0);
        isEnd = true;
        Async.StopAsync(this);
    }

    private void SmoothDampToDir(Vector3 dir)
    {
        if (monster.smoothDampToDir&&!isEnd)
        {
            float ttt = 0.01f;
            float fow = monster.animator.GetFloat("moveFoward");
            float left = monster.animator.GetFloat("moveLeft");


            var temp = Vector3.SmoothDamp(new Vector3(left, 0, fow), dir, ref smothFoward, monster.smoothDampTime);
            float forResult = temp.z;
            monster.animator.SetFloat("moveFoward", temp.z);
            //monster.animator.SetFloat("moveLeft", fow > ttt ? -temp.x : temp.x);
            monster.animator.SetFloat("moveLeft", temp.x);
        }
    }
}