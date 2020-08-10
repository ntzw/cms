import React, { useRef, useState, useEffect } from 'react'
import { Drawer, Button, Spin, message, Modal } from 'antd';
import ProTable, { ActionType as TableAction, ProColumns } from '@/components/ListTable';
import { ColumnContentItem } from '../data';
import { Column } from '../../columnlist/data';
import { RecyclePage, ContentDelete, ContentRemovedRecycle } from '../service';
import { ColumnField } from '@/components/Content/data';
import { lowerCaseFieldName } from '@/utils/utils';
import moment from 'moment';
import { DeleteOutlined, RollbackOutlined } from '@ant-design/icons';
import { DeleteConfirm } from '@/utils/msg';

interface RecycleListProps {
    visible: boolean;
    onClose: () => void;
    currentColumn?: Column;
    currentTableFields?: ColumnField[];
    afterVisibleChange?: (visible: boolean) => void;
}

const RecycleList: React.FC<RecycleListProps> = ({ visible, onClose, currentColumn, currentTableFields, afterVisibleChange }) => {
    const [visibleLoad, setVisibleLoad] = useState(false);
    const [loadColumns, setLoadColumns] = useState(false);
    const tableAction = useRef<TableAction>();
    const [contentTableColumns, setContentTableColumns] = useState<ProColumns<ColumnContentItem>[]>([]);

    useEffect(() => {
        if (currentTableFields && currentTableFields.length > 0) {
            setLoadColumns(true);
            const columns: ProColumns<ColumnContentItem>[] = currentTableFields.map((temp): ProColumns<ColumnContentItem> => {
                return {
                    dataIndex: lowerCaseFieldName(temp.name),
                    title: temp.explain,
                    ellipsis: true,
                    width: 100,
                };
            });

            columns.push({
                dataIndex: 'createDate',
                title: '创建时间',
                render: (value) => {
                    return moment(value + '').format('YYYY-MM-DD HH:mm');
                }
            });
            setContentTableColumns(columns);
            tableAction.current?.reload();
            setLoadColumns(false);
        }
    }, [currentTableFields])

    useEffect(() => {
        if (visible) {
            if (visibleLoad)
                tableAction.current?.reload();

            setVisibleLoad(true);
        }
    }, [visible])

    return <Drawer width="80%" visible={visible} onClose={() => { onClose(); }} placement="left" afterVisibleChange={afterVisibleChange} >
        <Spin spinning={loadColumns} tip="页面准备中....">
            <ProTable<ColumnContentItem>
                headerTitle={`${currentColumn?.name} 回收站列表`}
                actionRef={tableAction}
                request={(params, sort, query) => {
                    params['columnNum'] = currentColumn?.num;
                    return RecyclePage({
                        params,
                        sort,
                        query
                    });
                }}
                columns={contentTableColumns}
                rowSelection={{}}
                pagination={{
                    size: "default"
                }}
                options={{
                    fullScreen: false,
                }}
                params={{

                }}
                toolBarRender={(action, { selectedRowKeys, selectedRows }) => [
                    selectedRows && selectedRows.length > 0 && <Button
                        icon={<RollbackOutlined />}
                        type="primary"
                        onClick={() => {
                            Modal.confirm({
                                title: '系统提示',
                                content: '确定将选择的内容还原？',
                                okText: '确定还原',
                                cancelText: '取消',
                                onOk: () => {
                                    ContentRemovedRecycle(selectedRows.map(temp => temp.id), currentColumn?.num || '').then(res => {
                                        if (res.isSuccess) {
                                            message.success('还原成功');
                                            tableAction.current?.reload();
                                            tableAction.current?.clearSelected();
                                        } else {
                                            message.error(res.message || '还原失败');
                                        }
                                    })
                                }
                            })
                        }}
                    >还原</Button>,
                    selectedRows && selectedRows.length > 0 && <Button
                        icon={<DeleteOutlined />}
                        type="primary"
                        danger
                        onClick={() => {
                            DeleteConfirm(() => {
                                ContentDelete(selectedRows.map(temp => temp.id), currentColumn?.num || '').then(res => {
                                    if (res.isSuccess) {
                                        message.success('删除成功');
                                        tableAction.current?.reload();
                                    } else {
                                        message.error(res.message || '删除失败');
                                    }
                                })
                            })
                        }}
                    >删除</Button>,
                ]}
            />
        </Spin>
    </Drawer>
}

export default RecycleList;
