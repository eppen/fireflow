/**
 * Copyright 2007-2010 非也
 * All rights reserved. 
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation。
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see http://www.gnu.org/licenses. *
 */
package org.fireflow.pdl.fpdl20.test.service.javaexecutor;

import javax.jws.WebParam;
import javax.jws.WebResult;

/**
 * 
 * 
 * @author 非也
 * @version 2.0
 */
public interface IMathOperationBean {
	public @WebResult(name="result") int add(@WebParam(name="aa") int a,@WebParam(name="bb") int b);
	
	public int multiply(@WebParam(name="aa") int a,@WebParam(name="b")  int b);
}
