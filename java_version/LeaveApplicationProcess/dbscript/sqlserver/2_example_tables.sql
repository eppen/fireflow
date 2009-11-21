drop table T_Biz_EmailMock;
drop table T_Biz_LeaveApplicationInfo;
drop table T_Biz_LeaveApprovalInfo;
drop table T_FF_RT_TASKINSTANCE;
create table T_Biz_EmailMock (ID varchar(50) not null, user_Id varchar(50) not null, content varchar(512) not null, create_Time datetime not null, primary key (ID));
create table T_Biz_LeaveApplicationInfo (ID varchar(50) not null, sn varchar(50) not null unique, leaveReason varchar(50) null, fromDate varchar(50) null, toDate varchar(50) null, leaveDays int null, applicant_Id varchar(50) null, applicant_Name varchar(50) null, submitTime varchar(50) null, approval_Flag tinyint null, approval_Detail varchar(50) null, approver varchar(50) null, approval_Time datetime null, primary key (ID));
create table T_Biz_LeaveApprovalInfo (ID varchar(50) not null, sn varchar(50) not null, approver varchar(50) null, approval_Flag tinyint null, detail varchar(100) null, approval_Time datetime null, primary key (ID));
create table T_FF_RT_TASKINSTANCE (ID varchar(50) not null, BIZ_TYPE varchar(250) not null, TASK_ID varchar(300) not null, ACTIVITY_ID varchar(200) not null, NAME varchar(100) not null, DISPLAY_NAME varchar(128) null, STATE int not null, SUSPENDED tinyint not null, TASK_TYPE varchar(10) null, CREATED_TIME datetime not null, STARTED_TIME datetime null, EXPIRED_TIME datetime null, END_TIME datetime null, ASSIGNMENT_STRATEGY varchar(10) null, PROCESSINSTANCE_ID varchar(50) not null, PROCESS_ID varchar(100) not null, VERSION int not null, TARGET_ACTIVITY_ID varchar(100) null, FROM_ACTIVITY_ID varchar(600) null, STEP_NUMBER int not null, CAN_BE_WITHDRAWN tinyint not null, primary key (ID));
create index idx_taskInst_stepNb on T_FF_RT_TASKINSTANCE (STEP_NUMBER);
