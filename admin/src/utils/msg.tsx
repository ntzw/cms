import React from 'react';
import { Modal, Typography } from "antd";

const { Text } = Typography;

/**
 * 删除操作确认提示框
 * @param callback 确认操作回调
 */
export function DeleteConfirm(callback: () => void) {
    Modal.confirm({
        title: '系统提示',
        content: <Text strong><Text type="danger">删除操作不可逆</Text>，请谨慎操作！确定删除？</Text>,
        okText: '确定删除',
        cancelText: '取消',
        onOk: callback,
    })
}