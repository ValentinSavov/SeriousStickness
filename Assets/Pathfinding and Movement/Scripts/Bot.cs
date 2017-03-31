using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Algorithms;

public class Bot : Character
{
	public enum BotAction
	{
		None = 0,
		MoveTo,
	}
	
	public BotAction mCurrentAction = BotAction.None;
	
	public Vector2 mDestination;
	
	public int mCurrentNodeId = -1;

	public int mFramesOfJumping = 0;
	public int mStuckFrames = 0;

    public int mMaxJumpHeight = 5;
	
	
	public const int cMaxStuckFrames = 20;
    public const int cMaxFramesNotGettingCloserToNextNode = 120;
    public float mPrevDistanceToCurrentDestX, mPrevDistanceToCurrentDestY;
    public int mNotGettingCloserToNextNodeFrames = 0;

    bool mJumpingUpStairsLeft = false;
    bool mJumpingUpStairsRight = false;


    public void TappedOnTile(Vector2i mapPos)
    {
        //while (!(mMap.IsObstacle(mapPos.x, mapPos.y) || mMap.IsOneWayPlatform(mapPos.x, mapPos.y)))
        //    --mapPos.y;

        MoveTo(new Vector2i(mapPos.x, mapPos.y));
    }
    void Start()
    {
        // this simulates control over Character class just like user inputs
        CharacterInit(new bool[(int)KeyInput.Count], new bool[(int)KeyInput.Count]);
        BehaviorInit();
    }
    void CharacterInit(bool[] inputs, bool[] prevInputs)
    {
        mInputs = inputs;
        mPrevInputs = prevInputs;

        mMap = GameObject.FindObjectOfType<Map>();
        mAudioSource = GetComponent<AudioSource>();
        mPosition = transform.position;

        //demo1
        mAABB.HalfSize = new Vector2(6.0f, 7.0f);
        //demo2
        //mAABB.HalfSize = new Vector2(6.0f, 20.0f);

        mJumpSpeed = Constants.cJumpSpeed;
        mWalkSpeed = Constants.cWalkSpeed;

        //transform.localScale = new Vector3(mAABB.HalfSizeX / 8.0f, mAABB.HalfSizeY / 8.0f, 1.0f);
    }
    public void MoveTo(Vector2i destination)
    {
        mStuckFrames = 0;
        mNotGettingCloserToNextNodeFrames = 0;
        mPrevDistanceToCurrentDestX = 0.0f;
        mPrevDistanceToCurrentDestY = 0.0f;
        PathFinderFast mPathFinder = new PathFinderFast(mMap);
        OnFoundPath(
            mPathFinder.FindPath(
                    mMap.GetMapTileAtPoint(new Vector2(mAABB.Center.x, mAABB.Center.y - mAABB.HalfSizeY + 1.0f)), 
                    new Vector2i(destination.x, destination.y),
                    Mathf.CeilToInt(mAABB.HalfSizeX / 8.0f), Mathf.CeilToInt(mAABB.HalfSizeY / 8.0f), 
                    (short)mMaxJumpHeight));
    }
    void OnFoundPath(List<Vector2i> path)
    {
        mJumpingUpStairsRight = false;
        mJumpingUpStairsLeft = false;

        mPath.Clear();

        if (path != null && path.Count > 1)
        {
            for (var i = path.Count - 1; i >= 0; --i)
                mPath.Add(path[i]);

            mCurrentNodeId = 1;

            if (mCurrentNodeId < mPath.Count)
                mFramesOfJumping = GetJumpFrameCount(mPath[mCurrentNodeId].y - mPath[mCurrentNodeId - 1].y);

            ChangeAction(BotAction.MoveTo);
        }
        else
        {
            mCurrentNodeId = -1;

            if (mCurrentAction == BotAction.MoveTo)
                mCurrentAction = BotAction.None;
        }

        if (!Debug.isDebugBuild)
            DrawPathLines();
    }
    void MoveTo(Vector2 destination)
    {
        MoveTo(mMap.GetMapTileAtPoint(destination));
    }
    void ChangeAction(BotAction newAction)
    {
        mCurrentAction = newAction;
    }
    int GetJumpFrameCount(int deltaY)
    {
        if (deltaY <= 0)
            return 0;
        else
        {
            switch (deltaY)
            {
                case 1:
                    return 1;
                case 2:
                    return 2;
                case 3:
                    return 5;
                case 4:
                    return 8;
                case 5:
                    return 14;
                default:
                    return 30;
            }
        }
    }
    void UpdateDest(out Vector2 prevDest, out Vector2 currentDest, out Vector2 nextDest, out bool destOnGround, out Vector2 pathPosition, out bool reachedY, out bool reachedX, int characterHeight, int prevNodeOffset)
    {
        prevDest = new Vector2(mPath[mCurrentNodeId - prevNodeOffset].x * Map.cTileSize + mMap.transform.position.x,
                                             mPath[mCurrentNodeId - prevNodeOffset].y * Map.cTileSize + mMap.transform.position.y);
        currentDest = new Vector2(mPath[mCurrentNodeId].x * Map.cTileSize + mMap.transform.position.x,
                                          mPath[mCurrentNodeId].y * Map.cTileSize + mMap.transform.position.y);
        nextDest = currentDest;

        if (mPath.Count > mCurrentNodeId + 1)
        {
            nextDest = new Vector2(mPath[mCurrentNodeId + 1].x * Map.cTileSize + mMap.transform.position.x,
                                          mPath[mCurrentNodeId + 1].y * Map.cTileSize + mMap.transform.position.y);
        }

        destOnGround = mMap.IsObstacle(mPath[mCurrentNodeId].x, mPath[mCurrentNodeId].y - 1)
            || mMap.IsOneWayPlatform(mPath[mCurrentNodeId].x, mPath[mCurrentNodeId].y - 1);

        pathPosition = mAABB.Center - mAABB.HalfSize + Vector2.one * Map.cTileSize * 0.5f;

        reachedX = (prevDest.x <= currentDest.x && pathPosition.x >= currentDest.x)
            || (prevDest.x >= currentDest.x && pathPosition.x <= currentDest.x);
        reachedX = reachedX || Mathf.Abs(pathPosition.x - currentDest.x) <= Constants.cBotMaxPositionError;

        if (reachedX && Mathf.Abs(pathPosition.x - currentDest.x) > Constants.cBotMaxPositionError 
            && Mathf.Abs(pathPosition.x - currentDest.x) < Map.cTileSize 
            && !mPrevInputs[(int)KeyInput.GoRight] && !mPrevInputs[(int)KeyInput.GoLeft])
        {
            if (Mathf.Abs(pathPosition.x - nextDest.x) < Mathf.Abs(pathPosition.x - currentDest.x))
                mPosition.x = nextDest.x;
            else
                mPosition.x = currentDest.x;
        }

        reachedY = (prevDest.y <= currentDest.y && pathPosition.y - currentDest.y >= 0.0f && pathPosition.y - currentDest.y <= Map.cTileSize * 2)
            || (prevDest.y >= currentDest.y && pathPosition.y <= currentDest.y);
        reachedY = reachedY || Mathf.Abs(pathPosition.y - currentDest.y) <= Constants.cBotMaxPositionError;
        //bool reachedY = (pathPosition.y >= currentDest.y);

        if (destOnGround && !mOnGround)
            reachedY = false;

        if (!reachedY && reachedX && currentDest.y > pathPosition.y && !mOnGround && mSpeed.y < 0.0f)
            reachedY = true;
    }
    void Stop()
    {
        ChangeAction(BotAction.None);
    }

    void FixedUpdate()
    {
        BotUpdate();
    }
	void BotUpdate()
	{
        mInputs[(int)KeyInput.GoRight] = false;
        mInputs[(int)KeyInput.GoLeft] = false;
        mInputs[(int)KeyInput.Jump] = false;
        mInputs[(int)KeyInput.GoDown] = false;

        //get the position of the bottom of the bot's aabb, this will be much more useful than the center of the sprite (mPosition)
        int tileX, tileY;
        var position = mAABB.Center;
        position.y -= mAABB.HalfSizeY;

		mMap.GetMapTileAtPoint(position, out tileX, out tileY);
		
		int characterHeight = Mathf.CeilToInt(mAABB.HalfSizeY*2.0f/Map.cTileSize);

        int dir;

        switch (mCurrentAction)
        {
            case BotAction.None:


                if (mFramesOfJumping > 0)
                {
                    mFramesOfJumping -= 1;
                    mInputs[(int)KeyInput.Jump] = true;
                }

                break;

            case BotAction.MoveTo:

                Vector2 prevDest, currentDest, nextDest, pathPosition;
                bool destOnGround, reachedY, reachedX;
                UpdateDest(out prevDest, out currentDest, out nextDest, out destOnGround, out pathPosition, out reachedY, out reachedX, characterHeight, 1);

                mInputs[(int)KeyInput.GoRight] = false;
                mInputs[(int)KeyInput.GoLeft] = false;
                mInputs[(int)KeyInput.Jump] = false;
                mInputs[(int)KeyInput.GoDown] = false;

                if (pathPosition.y - currentDest.y > Constants.cBotMaxPositionError && mOnOneWayPlatform)
                    mInputs[(int)KeyInput.GoDown] = true;

                if (reachedX && reachedY)
                {
                    int prevNodeId = mCurrentNodeId;

                    mJumpingUpStairsRight = false;
                    mJumpingUpStairsLeft = false;

                    //handle  the stairs case
                    if (destOnGround)
                    {
                        for (int i = 6; i > 0 && !mJumpingUpStairsRight && !mJumpingUpStairsRight; i -= 3)
                        {
                            if (mCurrentNodeId + i < mPath.Count)
                            {
                                int oldX = -1, oldY = -1;
                                dir = mPath[mCurrentNodeId].x > mPath[mCurrentNodeId - 1].x ? 1 : -1;

                                bool isStair = true;
                                for (int j = i; j >= 0; j -= 3)
                                {
                                    if (!mMap.IsNotEmpty(mPath[mCurrentNodeId + j].x, mPath[mCurrentNodeId + j].y - 1)
                                        || !mMap.IsObstacle(mPath[mCurrentNodeId + j].x + dir, mPath[mCurrentNodeId + j].y)
                                        || (oldX != -1 && (mPath[mCurrentNodeId + j].y + 1 != oldY || mPath[mCurrentNodeId + j].x + dir != oldX)))
                                    {
                                        isStair = false;
                                        break;
                                    }

                                    oldX = mPath[mCurrentNodeId + j].x;
                                    oldY = mPath[mCurrentNodeId + j].y;
                                }

                                if (isStair)
                                {
                                    mCurrentNodeId += i;
                                    if (dir == 1)
                                        mJumpingUpStairsRight = true;
                                    else
                                        mJumpingUpStairsLeft = true;

                                    break;
                                }
                            }
                        }
                    }

                    if (!mJumpingUpStairsLeft && !mJumpingUpStairsRight)
                    {
                        mCurrentNodeId++;
                    }

                    if (mCurrentNodeId >= mPath.Count)
                    {
                        mCurrentNodeId = -1;
                        ChangeAction(BotAction.None);
                        break;
                    }

                    dir = mPath[mCurrentNodeId].x > mPath[prevNodeId].x ? 1 : -1;

                    if (!mMap.AnySolidBlockInStripe(mPath[prevNodeId].x + dir, tileY + characterHeight, mPath[mCurrentNodeId].y))
                    {
                        if (mPath[mCurrentNodeId].x > mPath[prevNodeId].x)
                            mInputs[(int)KeyInput.GoRight] = true;
                        else if (mPath[mCurrentNodeId].x < mPath[prevNodeId].x)
                            mInputs[(int)KeyInput.GoLeft] = true;
                    }

                    if (!mOnGround && mPath[mCurrentNodeId].y == mPath[prevNodeId].y)
                        mFramesOfJumping = 1;
                    else
                        mFramesOfJumping = GetJumpFrameCount(mPath[mCurrentNodeId].y - mPath[prevNodeId].y);

                    UpdateDest(out prevDest, out currentDest, out nextDest, out destOnGround, out pathPosition, out reachedY, out reachedX, characterHeight, mCurrentNodeId - prevNodeId);
                }
                else if (mJumpingUpStairsRight || mJumpingUpStairsLeft)
                {
                    if (currentDest.x > pathPosition.x && mJumpingUpStairsRight && !reachedX)
                        mInputs[(int)KeyInput.GoRight] = true;
                    else if (currentDest.x < pathPosition.x && mJumpingUpStairsLeft && !reachedX)
                        mInputs[(int)KeyInput.GoLeft] = true;

                    if (mOnGround)
                    {
                        mFramesOfJumping = GetJumpFrameCount(mPath[mCurrentNodeId].y - tileY);
                        if (mFramesOfJumping == 0)
                            mFramesOfJumping = 1;
                    }
                }
                else
                {
                    if (!reachedX)
                    {
                        if (destOnGround && currentDest.y > pathPosition.y && currentDest.y - pathPosition.y > mAABB.HalfSizeY * 2.0f)
                        {
                            //do nothing
                        }
                        else
                        {
                            if (currentDest.x > pathPosition.x /*&& !mPushesRightWall*/)
                                mInputs[(int)KeyInput.GoRight] = true;
                            else if (currentDest.x < pathPosition.x /*&& !mPushesLeftWall*/)
                                mInputs[(int)KeyInput.GoLeft] = true;
                        }
                    }
                    else if (!reachedY)
                    {
                        if (mPath.Count > mCurrentNodeId + 1 && !destOnGround)
                        {
                            dir = mPath[mCurrentNodeId + 1].x - mPath[mCurrentNodeId].x > 0 ? 1 : -1;

                            if (nextDest.y < currentDest.y)
                            {
                                if (mPath[mCurrentNodeId].y > mPath[mCurrentNodeId + 1].y
                                    && mPath[mCurrentNodeId].x != mPath[mCurrentNodeId + 1].x
                                    && !mMap.AnySolidBlockInStripe(tileX + dir, tileY, mPath[mCurrentNodeId + 1].y))
                                {
                                    if (nextDest.x > pathPosition.x && currentDest.x > prevDest.x)
                                        mInputs[(int)KeyInput.GoRight] = true;
                                    else if (nextDest.x < pathPosition.x && currentDest.x < prevDest.x)
                                        mInputs[(int)KeyInput.GoLeft] = true;
                                }
                            }
                            else if (prevDest.y <= currentDest.y
                                && currentDest.y <= nextDest.y
                                && !mMap.IsNotEmpty(mPath[mCurrentNodeId].x, mPath[mCurrentNodeId].y - 1)
                                //&& !mMap.IsGround(mPath[mCurrentNodeId + 1].x, mPath[mCurrentNodeId + 1].y - 1)
                                //&& !mMap.AnySolidBlockInRectangle(mPath[mCurrentNodeId - 1], mPath[mCurrentNodeId])
                                && !mMap.AnySolidBlockInStripe(tileX + dir, tileY + characterHeight - 1, mPath[mCurrentNodeId + 1].y))
                            {
                                if (nextDest.x > pathPosition.x)
                                    mInputs[(int)KeyInput.GoRight] = true;
                                else if (nextDest.x < pathPosition.x)
                                    mInputs[(int)KeyInput.GoLeft] = true;
                            }
                        }

                        if (mFramesOfJumping == 0
                            && currentDest.y - pathPosition.y > Constants.cBotMaxPositionError
                            && ((mWasOnGround && !mOnGround)
                                || tileX == mPath[mCurrentNodeId].x
                                || mPushesLeftWall
                                || mPushesRightWall))
                        {
                            mFramesOfJumping = GetJumpFrameCount(mPath[mCurrentNodeId].y - mPath[mCurrentNodeId - 1].y);
                        }
                    }
                }

                if (mFramesOfJumping > 0)
                {
                    if (!destOnGround && currentDest.y >= pathPosition.y && !reachedX && mOnGround)
                    {
                        //do nothing
                    }
                    else
                    {
                        mInputs[(int)KeyInput.Jump] = true;
                        --mFramesOfJumping;

                        if (!(mJumpingUpStairsLeft || mJumpingUpStairsRight) && mFramesOfJumping == 0 && ((!reachedX && !mOnGround) || (reachedX && !reachedY && !mOnGround && currentDest.y > pathPosition.y)))
                            mFramesOfJumping++;
                    }
                }

                if (mCurrentState == Character.CharacterState.GrabLedge)
                {
                    mInputs[(int)KeyInput.GoDown] = true;
                    mInputs[(int)KeyInput.GoRight] = false;
                    mInputs[(int)KeyInput.GoLeft] = false;
                }

                if (Mathf.Abs(pathPosition.x - currentDest.x) < mPrevDistanceToCurrentDestX
                    || Mathf.Abs(pathPosition.y - currentDest.y) < mPrevDistanceToCurrentDestY)
                {
                    ++mNotGettingCloserToNextNodeFrames;

                    if (+mNotGettingCloserToNextNodeFrames > cMaxFramesNotGettingCloserToNextNode)
                        MoveTo(mPath[mPath.Count - 1]);
                }
                else
                    mNotGettingCloserToNextNodeFrames = 0;
                mPrevDistanceToCurrentDestX = Mathf.Abs(pathPosition.x - currentDest.x);
                mPrevDistanceToCurrentDestY = Mathf.Abs(pathPosition.y - currentDest.y);

                if (mPosition == mOldPosition)
                {
                    ++mStuckFrames;
                    if (mStuckFrames > cMaxStuckFrames)
                        MoveTo(mPath[mPath.Count - 1]);
                }
                else
                    mStuckFrames = 0;

                break;
        }
			
        
        if (gameObject.activeInHierarchy)
		    CharacterUpdate();
	}


#region States AI Stuff
    private FSM stateMachine;
    private FSM.FSMState idleState; // finds something to do
    private FSM.FSMState moveToState; // moves to a target
    private FSM.FSMState performActionState; // performs an action
    void BehaviorInit()
    {
        //stateMachine = new FSM();
        //createIdleState();
        //createMoveToState();
        //createPerformActionState();
        //stateMachine.pushState(idleState);
    }

    void ResetFSM()
    {
        stateMachine = new FSM();
        stateMachine.pushState(idleState);
    }

    private void createIdleState()
    {
        idleState = (fsm, gameObj) => {
            // GOAP planning

            // get the world state and the goal we want to plan for
            //HashSet<KeyValuePair<string, object>> worldState = dataProvider.getWorldState();
            //HashSet<KeyValuePair<string, object>> goal = dataProvider.createGoalState();

            // Plan
            //Queue<GoapAction> plan = planner.plan(gameObject, availableActions, worldState, goal);
            //if (plan != null)
            {
                // we have a plan, hooray!
                //currentActions = plan;
                //dataProvider.planFound(goal, plan);

                fsm.popState(); // move to PerformAction state
                fsm.pushState(performActionState);

            }
            //else
            {
                // ugh, we couldn't get a plan
                //Debug.Log("<color=orange>Failed Plan:</color>" + prettyPrint(goal));
                //dataProvider.planFailed(goal);
                fsm.popState(); // move back to IdleAction state
                fsm.pushState(idleState);
            }
        };
    }

    private void createMoveToState()
    {
        moveToState = (fsm, gameObj) => {
            // move the game object

            //GoapAction action = currentActions.Peek();
            //if (action.requiresInRange() && action.target == null)
            {
                Debug.Log("<color=red>Fatal error:</color> Action requires a target but has none. Planning failed. You did not assign the target in your Action.checkProceduralPrecondition()");
                fsm.popState(); // move
                fsm.popState(); // perform
                fsm.pushState(idleState);
                return;
            }

            // get the agent to go to action spot
            //if (dataProvider.GoToAction(action))
            {
                fsm.popState();
            }
        };
    }

    private void createPerformActionState()
    {
        performActionState = (fsm, gameObj) => {
            // perform the action

            //if (!hasActionPlan())
            {
                // no actions to perform
                Debug.Log("<color=red>Done actions</color>");
                fsm.popState();
                fsm.pushState(idleState);
                //dataProvider.actionsFinished();
                return;
            }

            //GoapAction action = currentActions.Peek();
            //if (action.isDone())
            {
                // the action is done. Remove it so we can perform the next one
                //currentActions.Dequeue();
            }

            //if (hasActionPlan())
            {
                // perform the next action
                //action = currentActions.Peek();
                //bool inRange = action.requiresInRange() ? action.isInRange() : true;

                //if (inRange)
                {
                    // we are in range, so perform the action
                    //bool success = action.perform(gameObj);

                    //if (!success)
                    {
                        // action failed, we need to plan again
                        fsm.popState();
                        fsm.pushState(idleState);
                        //dataProvider.planAborted(action);
                    }
                }
                //else
                {
                    // we need to move there first
                    // push moveTo state
                    fsm.pushState(moveToState);
                }

            }
            //else
            {
                // no actions left, move to Plan state
                fsm.popState();
                fsm.pushState(idleState);
                //dataProvider.actionsFinished();
            }

        };
    }
    #endregion
}