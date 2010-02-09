﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISM.FireWorkflow.Engine;
using ISM.FireWorkflow.Engine.Definition;
using ISM.FireWorkflow.Engine.Event;
using ISM.FireWorkflow.Engine.Persistence;
using ISM.FireWorkflow.Kernel;
using ISM.FireWorkflow.Kernel.Impl;
using ISM.FireWorkflow.Model;

namespace ISM.FireWorkflow.Engine.Impl
{
    /// <summary>ProcessInstance generated by hbm2java</summary>
    [Serializable]
    public class ProcessInstance : IProcessInstance, IRuntimeContextAware, IWorkflowSessionAware
    {
        private String id = null;
        private String processId = null;
        private Int32 version ;
        private String name = null;
        private String displayName = null;
        private Int32 state;
        private Boolean? suspended = null;
        private String creatorId = null;
        private DateTime? createdTime = null;
        private DateTime? startedTime = null;
        private DateTime? endTime = null;
        private DateTime? expiredTime = null;
        private String parentProcessInstanceId = null;
        private String parentTaskInstanceId = null;
        private Dictionary<String, Object> processInstanceVariables = new Dictionary<String, Object>();
        [NonSerialized]
        protected RuntimeContext rtCtx = null;
        [NonSerialized]
        protected IWorkflowSession workflowSession = null;

        public void setRuntimeContext(RuntimeContext ctx)
        {
            this.rtCtx = ctx;
        }

        public RuntimeContext getRuntimeContext()
        {
            return this.rtCtx;
        }

        public ProcessInstance()
        {
            this.state = IProcessInstance.INITIALIZED;
            this.suspended = false;
        }

        public override String getId()
        {
            return this.id;
        }

        public void setId(String id)
        {
            this.id = id;
        }

        public override String getProcessId()
        {
            return this.processId;
        }

        public void setProcessId(String processID)
        {
            this.processId = processID;
        }

        public override Int32 getVersion()
        {
            return version;
        }

        public void setVersion(Int32 version)
        {
            this.version = version;
        }

        public override String getName()
        {
            return this.name;
        }

        public void setName(String name)
        {
            this.name = name;
        }

        public override String getDisplayName()
        {
            return this.displayName;
        }

        public void setDisplayName(String label)
        {
            this.displayName = label;
        }

        public override Int32 getState()
        {
            return this.state;
        }

        public void setState(Int32 state)
        {
            this.state = state;
        }

        public override String getParentProcessInstanceId()
        {
            return parentProcessInstanceId;
        }

        public void setParentProcessInstanceId(String parentProcessInstanceId)
        {
            this.parentProcessInstanceId = parentProcessInstanceId;
        }

        public IJoinPoint createJoinPoint(ISynchronizerInstance synchInst, IToken token)// throws EngineException 
        {

            int enterTransInstanceCount = synchInst.getEnteringTransitionInstances().Count;
            if (enterTransInstanceCount == 0)
            {

                throw new EngineException(this.getId(), this.getWorkflowProcess(),
                        synchInst.getSynchronizer().getId(), "The process definition [" + this.getName() + "] is invalid，the synchronizer[" + synchInst.getSynchronizer() + "] has no entering transition");
            }
            IPersistenceService persistenceService = rtCtx.getPersistenceService();
            //保存到数据库
            persistenceService.saveOrUpdateToken(token);

            IJoinPoint resultJoinPoint = null;
            resultJoinPoint = new JoinPoint();
            resultJoinPoint.setProcessInstance(this);
            resultJoinPoint.setSynchronizerId(synchInst.getSynchronizer().getId());
            if (enterTransInstanceCount == 1)
            {
                // 生成一个不存储到数据库中的JoinPoint
                resultJoinPoint.addValue(token.getValue());

                if (token.isAlive())
                {
                    resultJoinPoint.setAlive(true);
                    resultJoinPoint.setFromActivityId(token.getFromActivityId());
                }
                resultJoinPoint.setStepNumber(token.getStepNumber() + 1);

                return resultJoinPoint;
            }
            else
            {

                int stepNumber = 0;

                List<IToken> tokensList_0 = persistenceService.findTokensForProcessInstance(this.getId(), synchInst.getSynchronizer().getId());
                Dictionary<String, IToken> tokensMap = new Dictionary<String, IToken>();
                for (int i = 0; i < tokensList_0.Count; i++)
                {
                    IToken tmpToken = (IToken)tokensList_0[i];
                    String tmpFromActivityId = tmpToken.getFromActivityId();
                    if (!tokensMap.ContainsKey(tmpFromActivityId))
                    {
                        tokensMap.Add(tmpFromActivityId, tmpToken);
                    }
                    else
                    {
                        IToken tmpToken2 = (IToken)tokensMap[tmpFromActivityId];
                        if (tmpToken2.getStepNumber() > tmpToken.getStepNumber())
                        {
                            tokensMap[tmpFromActivityId] = tmpToken2;
                        }
                    }
                }

                List<IToken> tokensList = new List<IToken>(tokensMap.Values);

                for (int i = 0; i < tokensList.Count; i++)
                {
                    IToken _token = (IToken)tokensList[i];
                    resultJoinPoint.addValue(_token.getValue());
                    if (_token.isAlive())
                    {
                        resultJoinPoint.setAlive(true);
                        String oldFromActivityId = resultJoinPoint.getFromActivityId();
                        if (oldFromActivityId == null || oldFromActivityId.Trim().Equals(""))
                        {
                            resultJoinPoint.setFromActivityId(_token.getFromActivityId());
                        }
                        else
                        {
                            resultJoinPoint.setFromActivityId(oldFromActivityId + IToken.FROM_ACTIVITY_ID_SEPARATOR + _token.getFromActivityId());
                        }

                    }
                    if (token.getStepNumber() > stepNumber)
                    {
                        stepNumber = token.getStepNumber();
                    }
                }

                resultJoinPoint.setStepNumber(stepNumber + 1);

                return resultJoinPoint;
            }
            /*
            if (enterTransInstanceCount == 1) {
            // 生成一个不存储到数据库中的JoinPoint
            resultJoinPoint = new JoinPoint();
            resultJoinPoint.addValue(token.getValue());

            if (token.isAlive()) {
            resultJoinPoint.setAlive(true);
            }
            resultJoinPoint.setProcessInstance(this);
            return resultJoinPoint;
            } else {
            // 1、首先从数据库中查询，看看是否已经存在该JoinPoint
            //对于循环的情况,此处会存在一些问题...
            IPersistenceService persistenceService = rtCtx.getPersistenceService();
            List<IJoinPoint> joinPointList = persistenceService.findJoinPointsForProcessInstance(this.getId(), synchInst.getSynchronizer().getId());
            IJoinPoint joinPoint = null;
            if (joinPointList != null && joinPointList.Count > 0) {
            joinPoint = joinPointList.get(0);
            }
            if (joinPoint != null) {
            resultJoinPoint = joinPoint;
            resultJoinPoint.setProcessInstance(this);
            } else {
            // 2、生成一个存储到数据库中的joinPoint
            resultJoinPoint = new JoinPoint();
            resultJoinPoint.setProcessInstance(this);
            resultJoinPoint.setSynchronizerId(synchInst.getSynchronizer().getId());
            }
            resultJoinPoint.addValue(token.getValue());

            if (token.isAlive()) {
            resultJoinPoint.setAlive(true);
            }

            persistenceService.saveOrUpdateJoinPoint(resultJoinPoint);
            return resultJoinPoint;
            }
             */
        }

        public override void run()
        {
            if (this.getState() != IProcessInstance.INITIALIZED)
            {
                throw new EngineException(this.getId(),
                        this.getWorkflowProcess(),
                        this.getProcessId(), "The state of the process instance is " + this.getState() + ",can not run it ");
            }

            INetInstance netInstance = (INetInstance)rtCtx.getKernelManager().getNetInstance(this.getProcessId(), this.getVersion());
            if (netInstance == null)
            {
                throw new EngineException(this.getId(),
                        this.getWorkflowProcess(),
                        this.getProcessId(), "The net instance for the  workflow process [Id=" + this.getProcessId() + "] is Not found");
            }
            //触发事件
            ProcessInstanceEvent pevent = new ProcessInstanceEvent();
            pevent.setEventType(ProcessInstanceEvent.BEFORE_PROCESS_INSTANCE_RUN);
            pevent.setSource(this);
            this.fireProcessInstanceEvent(pevent);

            this.setState(IProcessInstance.RUNNING);
            this.setStartedTime(rtCtx.getCalendarService().getSysDate());
            rtCtx.getPersistenceService().saveOrUpdateProcessInstance(this);
            netInstance.run(this);
        }

        public override Dictionary<String, Object> getProcessInstanceVariables()
        {
            return processInstanceVariables;
        }

        public override void setProcessInstanceVariables(Dictionary<String, Object> vars)
        {
            processInstanceVariables = vars;
            //        processInstanceVariables.putAll(vars);
        }

        public override Object getProcessInstanceVariable(String name)
        {
            return processInstanceVariables[name];
        }

        public override void setProcessInstanceVariable(String name, Object var)
        {
            processInstanceVariables.Add(name, var);
        }

        public override WorkflowProcess getWorkflowProcess()
        {
            WorkflowDefinition workflowDef = rtCtx.getDefinitionService().getWorkflowDefinitionByProcessIdAndVersionNumber(this.getProcessId(), this.getVersion());
            WorkflowProcess workflowProcess = null;

            workflowProcess = workflowDef.getWorkflowProcess();


            return workflowProcess;
        }

        public override String getParentTaskInstanceId()
        {
            return parentTaskInstanceId;
        }

        public  void setParentTaskInstanceId(String taskInstId)
        {
            parentTaskInstanceId = taskInstId;
        }

        public override DateTime? getCreatedTime()
        {
            return this.createdTime;
        }

        public override DateTime? getStartedTime()
        {
            return this.startedTime;
        }

        public override DateTime? getEndTime()
        {
            return this.endTime;
        }

        public void setCreatedTime(DateTime createdTime)
        {
            this.createdTime = createdTime;
        }

        public void setEndTime(DateTime endTime)
        {
            this.endTime = endTime;
        }

        public void setStartedTime(DateTime startedTime)
        {
            this.startedTime = startedTime;
        }

        /**
         * 正常结束工作流
         * 1、首先检查有无活动的token,如果有则直接返回，如果没有则结束当前流程
         * 2、执行结束流程的操作，将state的值设置为结束状态
         * 3、然后检查parentTaskInstanceId是否为null，如果不为null则，调用父taskinstance的complete操作。
         */
        public void complete()
        {
            List<IToken> tokens = rtCtx.getPersistenceService().findTokensForProcessInstance(this.getId(), null);
            Boolean canBeCompleted = true;
            for (int i = 0; tokens != null && i < tokens.Count; i++)
            {
                IToken token = tokens[i];
                if (token.isAlive())
                {
                    canBeCompleted = false;
                    break;
                }
            }
            if (!canBeCompleted)
            {
                return;
            }

            this.setState(IProcessInstance.COMPLETED);
            //记录结束时间
            this.setEndTime(rtCtx.getCalendarService().getSysDate());
            rtCtx.getPersistenceService().saveOrUpdateProcessInstance(this);

            //触发事件
            ProcessInstanceEvent pevent = new ProcessInstanceEvent();
            pevent.setEventType(ProcessInstanceEvent.AFTER_PROCESS_INSTANCE_COMPLETE);
            pevent.setSource(this);
            this.fireProcessInstanceEvent(pevent);
            if (this.getParentTaskInstanceId() != null && !this.getParentTaskInstanceId().Trim().Equals(""))
            {
                ITaskInstance taskInstance = rtCtx.getPersistenceService().findAliveTaskInstanceById(this.getParentTaskInstanceId());
                ((IRuntimeContextAware)taskInstance).setRuntimeContext(rtCtx);
                ((IWorkflowSessionAware)taskInstance).setCurrentWorkflowSession(workflowSession);
                ((TaskInstance)taskInstance).complete(null);
            }
        }

        public override void abort()
        {
            if (this.state == IProcessInstance.COMPLETED || this.state == IProcessInstance.CANCELED)
            {
                throw new EngineException(this, this.getWorkflowProcess(), "The process instance can not be aborted,the state of this process instance is " + this.getState());
            }
            IPersistenceService persistenceService = rtCtx.getPersistenceService();
            persistenceService.abortProcessInstance(this);
        }


        /**
         * 触发process instance相关的事件
         * @param e
         * @throws org.fireflow.engine.EngineException
         */
        protected void fireProcessInstanceEvent(ProcessInstanceEvent e)
        {
            WorkflowProcess workflowProcess = this.getWorkflowProcess();
            if (workflowProcess == null)
            {
                return;
            }

            List<EventListener> listeners = workflowProcess.getEventListeners();
            for (int i = 0; i < listeners.Count; i++)
            {
                EventListener listener = (EventListener)listeners[i];
                Object obj = rtCtx.getBeanByName(listener.getClassName());
                if (obj != null)
                {
                    ((IProcessInstanceEventListener)obj).onProcessInstanceEventFired(e);
                }
            }
        }

        public override DateTime? getExpiredTime()
        {
            return this.expiredTime;
        }

        public  void setExpiredTime(DateTime arg)
        {
            this.expiredTime = arg;

        }

        public  IWorkflowSession getCurrentWorkflowSession()
        {
            return this.workflowSession;
        }

        public  void setCurrentWorkflowSession(IWorkflowSession session)
        {
            this.workflowSession = session;
        }

        public override String getCreatorId()
        {
            return this.creatorId;
        }

        public  void setCreatorId(String s)
        {
            this.creatorId = s;
        }

        public override  Boolean? isSuspended()
        {
            return suspended;
        }

        public  Boolean? getSuspended()
        {
            return suspended;
        }

        public  void setSuspended(Boolean isSuspended)
        {
            this.suspended = isSuspended;
        }

        public override void suspend()
        {
            if (this.state == IProcessInstance.COMPLETED || this.state == IProcessInstance.CANCELED)
            {
                throw new EngineException(this, this.getWorkflowProcess(), "The process instance can not be suspended,the state of this process instance is " + this.state);
            }
            if (this.isSuspended() != null && (bool)this.isSuspended())
            {
                return;
            }
            IPersistenceService persistenceService = this.rtCtx.getPersistenceService();
            persistenceService.suspendProcessInstance(this);
        }

        public override void restore()
        {
            if (this.state == IProcessInstance.COMPLETED || this.state == IProcessInstance.CANCELED)
            {
                throw new EngineException(this, this.getWorkflowProcess(), "The process instance can not be restored,the state of this process instance is " + this.state);
            }
            if (!(this.isSuspended() != null && (bool)this.isSuspended()))
            {
                return;
            }

            IPersistenceService persistenceService = this.rtCtx.getPersistenceService();
            persistenceService.restoreProcessInstance(this);

        }

    }
}
