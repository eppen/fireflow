package org.fireflow.pdl.fpdl20.io;

import java.io.IOException;
import java.io.OutputStream;

import org.fireflow.model.io.SerializerException;
import org.fireflow.pdl.fpdl20.process.WorkflowProcess;

/**
 * FPDL序列化器。将WorkflowProcess对象序列化到一个输出流。
 * @author 非也
 *
 */
public interface IFPDLSerializer extends FPDLNames {
	/**
	 * 将WorkflowProcess对象序列化到一个输出流。
	 * @param workflowProcess 工作流定义
	 * @param out 输出流
	 * @throws IOException
	 * @throws SerializerException
	 */
	public void serialize(WorkflowProcess workflowProcess, OutputStream out)
			throws IOException, SerializerException;

}
