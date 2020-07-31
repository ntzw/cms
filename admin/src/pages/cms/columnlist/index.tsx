import React, { useRef, useState } from 'react';
import { PageHeaderWrapper } from '@ant-design/pro-layout';
import ProTable, { ActionType as TableAction, ProColumns } from '@/components/ListTable';
import { ColumnListProps, Column, ColumnFieldListPropsState } from './data';
import { page, Submit, ClearColumnField, Delete } from './service';
import { Button, message, Tooltip, Popover, Alert, Popconfirm, Modal, Typography } from 'antd';
import ModalForm, { ModalFormState, ModalFormAction } from '@/components/ModalForm';
import { EditOutlined, DeleteOutlined, ControlOutlined, PlusOutlined } from '@ant-design/icons';
import moment from 'moment';
import { connect, GlobalModelState } from 'umi';
import ColumnFieldList from './components/columnfieldlist';
import { DeleteConfirm } from '@/utils/msg';

const { Text } = Typography

const DeleteColumns = (ids: number[], tableAction: React.MutableRefObject<TableAction | undefined>) => {
    Delete(ids).then(res => {
        if (res.isSuccess) {
            message.success('删除成功');
            tableAction.current?.reload();
        } else {
            message.error(res.message || '删除失败');
        }
    })
}

const ColumnList: React.FC<ColumnListProps> = ({ currentSite }) => {
    const [editForm, setEditForm] = useState<ModalFormState>({
        visible: false,
        title: '',
        isUpdate: false,
    });

    const [editColumnField, setEditColumnField] = useState<ColumnFieldListPropsState>({
        visible: false,
    })
    const [tableSelectedRows, setTableSelectedRows] = useState<Column[]>();

    const tableAction = useRef<TableAction>();
    const editAction = useRef<ModalFormAction>();
    const columns: ProColumns<Column>[] = [{
        dataIndex: 'name',
        title: '名称',
        valueType: 'option',
    }, {
        dataIndex: 'modelName',
        title: '模型名称',
        valueType: 'option',
    }, {
        dataIndex: 'createDate',
        title: '创建时间',
        valueType: 'option',
        render: (text) => {
            return typeof text === 'string' && text && moment(text).format('YYYY-MM-DD HH:mm:ss');
        }
    }, {
        dataIndex: 'num',
        title: '编号',
        valueType: 'option',
    }, {
        dataIndex: '-',
        title: '操作',
        valueType: 'option',
        render: (_, record) => {
            return <Button.Group>
                <Tooltip title="增加子栏目">
                    <Button
                        disabled={!!record.modelNum}
                        icon={<PlusOutlined />}
                        type="primary"
                        onClick={() => {
                            editAction.current?.clear();
                            editAction.current?.setOldValue();
                            setEditForm({
                                visible: true,
                                isUpdate: false,
                                title: '添加栏目',
                                params: {
                                    parentNum: record.num,
                                }
                            })
                        }}
                    />
                </Tooltip>
                <Tooltip title="编辑">
                    <Button
                        icon={<EditOutlined />}
                        onClick={() => {
                            setEditForm({
                                visible: true,
                                title: '编辑栏目',
                                isUpdate: true,
                                params: {
                                    id: record.id,
                                }
                            })
                        }}
                    />
                </Tooltip>
                <Tooltip title="删除">
                    <Button
                        icon={<DeleteOutlined />}
                        type="primary"
                        danger
                        onClick={() => {
                            DeleteColumns([record.id], tableAction);
                        }}
                    />
                </Tooltip>
                <Tooltip title="字段">
                    <Button
                        icon={<ControlOutlined />}
                        type="primary"
                        disabled={!record.modelNum}
                        onClick={() => {
                            setEditColumnField({
                                visible: true,
                                column: record,
                            })
                        }}
                    />
                </Tooltip>
            </Button.Group>
        }
    }];

    return <PageHeaderWrapper>
        <ProTable<Column>
            headerTitle="栏目列表"
            actionRef={tableAction}
            request={(params) => page(params)}
            columns={columns}
            rowSelection={{
                onChange: (selectedRowKeys, selectedRows) => {
                    setTableSelectedRows(selectedRows);
                }
            }}
            pagination={false}
            params={{
                siteNum: currentSite?.num,
            }}
            toolBarRender={(action) => {
                let tempModalNum = '';
                return [
                    <Button.Group>
                        <Button
                            icon={<PlusOutlined />}
                            type="primary"
                            onClick={() => {
                                editAction.current?.clear();
                                editAction.current?.setOldValue();
                                setEditForm({
                                    visible: true,
                                    isUpdate: false,
                                    title: '添加栏目'
                                })
                            }}
                        >
                            添加
                        </Button>
                        <Button
                            disabled={!tableSelectedRows || tableSelectedRows.length <= 0}
                            danger
                            type="primary"
                            icon={<DeleteOutlined />}
                            onClick={() => {
                                DeleteConfirm(() => {
                                    Modal.confirm({
                                        title: '删除提醒',
                                        content: <Text strong><Text style={{ color: 'red' }}>删除栏目，会清空栏目下所有内容</Text>，确定删除吗？</Text>,
                                        okText: '确定删除',
                                        cancelText: '取消',
                                        onOk: () => {
                                            if (tableSelectedRows) {
                                                DeleteColumns(tableSelectedRows.map(temp => temp.id), tableAction);
                                            }
                                        }
                                    })
                                })
                            }}
                        >
                            删除
                        </Button>
                        <Popconfirm title="批量设置字段，会先清空所选栏目原有字段，请谨慎操作!" okText="确定清空并添加" cancelText="取消" onConfirm={() => {
                            if (tableSelectedRows) {
                                ClearColumnField(tableSelectedRows.map(temp => temp.num)).then(res => {
                                    setEditColumnField({
                                        visible: true,
                                        column: tableSelectedRows,
                                    })
                                })
                            }
                        }}>
                            <Button
                                icon={<ControlOutlined />}
                                type="primary"
                                disabled={!tableSelectedRows || tableSelectedRows.length <= 0 || tableSelectedRows?.some(temp => {
                                    if (temp.modelNum) {
                                        if (!tempModalNum) {
                                            tempModalNum = temp.modelNum;
                                        } else if (temp.modelNum !== tempModalNum) {
                                            return true;
                                        }
                                    } else {
                                        return true;
                                    }

                                    return false;
                                })}
                            >
                                批量添加字段
                            </Button>
                        </Popconfirm>
                    </Button.Group>,
                ]
            }}
        />
        <ModalForm<Column>
            {...editForm}
            actionRef={editAction}
            fields="/api/CMS/Column/FormFields"
            onClose={() => {
                setEditForm({
                    ...editForm,
                    visible: false,
                })
            }}
            onFinish={(value) => {
                return new Promise(resolve => {
                    value.siteNum = currentSite?.num || '';
                    if (value.parentNum instanceof Array)
                        value.parentNum = value.parentNum[value.parentNum.length - 1];

                    Submit(value).then(res => {
                        resolve();
                        editAction.current?.reloadFieldItem();
                        if (res.isSuccess) {
                            message.success('操作成功');
                            tableAction.current?.reload();
                            setEditForm({
                                ...editForm,
                                visible: false,
                            })
                        } else {
                            message.error(res.message || '操作失败');
                        }
                    });
                })
            }}
            fieldActionParams={(field) => {
                switch (field.name.toLocaleLowerCase()) {
                    case 'parentnum':
                    case 'listtemplatepath':
                    case 'infotemplatepath':
                        return {
                            siteNum: currentSite?.num,
                        }
                }
                return null;
            }}
        />
        <ColumnFieldList
            {...editColumnField}
            onClose={() => {
                setEditColumnField({
                    ...editColumnField,
                    visible: false,
                })
            }}
        />
    </PageHeaderWrapper >
}

export default connect(({ global: { selectedSite } }: { global: GlobalModelState }) => {
    return {
        currentSite: selectedSite
    }
})(ColumnList);