import React, { useRef, useState, useEffect } from 'react'
import { ColumnFieldListProps, ModelFieldAddPropsState } from "../data";
import { Drawer, Row, Col, Button, Card, message, Modal, Tooltip } from 'antd';
import ProTable, { ActionType as TableAction, ProColumns } from '@/components/ListTable';
import { columnFieldPage, MoveModelField, DeleteColumnField } from '../service';
import { fieldPage } from '../../modeltablelist/service';
import ModelFieldAdd from './modelfieldadd';
import { PlusOutlined, SwapLeftOutlined, SwapRightOutlined, EditOutlined } from '@ant-design/icons';
import { GetFormItemTypeName } from '@/components/DynamicForm';
import { ModelField } from '../../modeltablelist/data';
import { AsyncContentFormAction, ColumnField } from '@/components/Content/data';
import AsyncContentForm from '@/components/Content/AsyncContentForm';


const ColumnFieldList: React.FC<ColumnFieldListProps> = ({ visible, onClose, column }) => {
    const [modelFieldAdd, setModelFieldAdd] = useState<ModelFieldAddPropsState>({
        visible: false,
        editType: 'model',
    });

    const [previewForm, setPreviewForm] = useState({
        visible: false,
        columnNum: '',
    })

    const [loading, setLoading] = useState({
        swapRight: false,
        swapLeft: false,
    })
    const previewFormAction = useRef<AsyncContentFormAction>();
    const exitsTableAction = useRef<TableAction>();
    const notExitsTableAction = useRef<TableAction>();
    const exitsColumns: ProColumns<ColumnField>[] = [{
        dataIndex: 'name',
        title: '名称',
        valueType: 'option',
    }, {
        dataIndex: 'explain',
        title: '说明',
        valueType: 'option',
    }, {
        dataIndex: 'optionType',
        title: '操作类型',
        valueType: 'option',
        render: (value) => {
            return GetFormItemTypeName(Number(value));
        }
    }, {
        dataIndex: '_',
        title: '操作',
        valueType: 'option',
        render: (value, record) => {
            return <Button.Group>
                <Tooltip title="编辑">
                    <Button
                        icon={<EditOutlined />}
                        onClick={() => {

                            setModelFieldAdd({
                                visible: true,
                                editId: record.id,
                                editType: 'column'
                            })
                        }}
                    />
                </Tooltip>
            </Button.Group>
        }
    }];

    const notExitsColumns: ProColumns<ModelField>[] = [{
        dataIndex: 'name',
        title: '名称',
        valueType: 'option',
    }, {
        dataIndex: 'explain',
        title: '说明',
        valueType: 'option',
    }, {
        dataIndex: 'optionType',
        title: '操作类型',
        valueType: 'option',
        render: (value) => {
            return GetFormItemTypeName(Number(value));
        }
    }, {
        dataIndex: '_',
        title: '操作',
        valueType: 'option',
        render: (value, record) => {
            return <Button.Group>
                <Tooltip title="编辑">
                    <Button
                        icon={<EditOutlined />}
                        onClick={() => {
                            setModelFieldAdd({
                                visible: true,
                                editId: record.id,
                                editType: 'model'
                            })
                        }}
                    />
                </Tooltip>
            </Button.Group>
        }
    }];

    const handleCancel = () => {
        onClose();
    }

    useEffect(() => {
        exitsTableAction.current?.reload();
        notExitsTableAction.current?.reload();
    }, [column])

    return <>
        <Drawer
            title={`栏目 ${column?.name} 字段管理`}
            width="98%"
            placement="left"
            visible={visible}
            onClose={handleCancel}
            bodyStyle={{ padding: 0 }}
        >
            <Row style={{ marginTop: 5 }} gutter={2}>
                <Col span={12}>
                    <Card bordered={true} bodyStyle={{ padding: 0 }}>
                        <ProTable<ColumnField>
                            headerTitle="已有字段"
                            options={{
                                fullScreen: false,
                                setting: false,
                            }}
                            actionRef={exitsTableAction}
                            request={(params, sort, query) => {
                                params['columnNum'] = column?.num;
                                return columnFieldPage({
                                    params,
                                    sort,
                                    query,
                                })
                            }}
                            columns={exitsColumns}
                            rowSelection={{}}
                            toolBarRender={(action, { selectedRows }) => [
                                <Button onClick={() => {
                                    setPreviewForm({
                                        visible: true,
                                        columnNum: column?.num || '',
                                    })
                                }} >预览</Button>,
                                selectedRows && selectedRows.length > 0 &&
                                <Button
                                    icon={<SwapRightOutlined />}
                                    type="primary"
                                    loading={loading.swapRight}
                                    onClick={() => {
                                        Modal.confirm({
                                            title: '系统提示',
                                            content: '确定将选择的字段移出？',
                                            okText: '确定移出',
                                            cancelText: '取消',
                                            onOk: () => {
                                                setLoading({
                                                    ...loading,
                                                    swapRight: true,
                                                })
                                                DeleteColumnField(selectedRows.map(temp => temp.id.toString())).then(res => {
                                                    setLoading({
                                                        ...loading,
                                                        swapRight: false,
                                                    })
                                                    if (res.isSuccess) {
                                                        message.success('移出成功');
                                                        exitsTableAction.current?.reload();
                                                        exitsTableAction.current?.clearSelected();
                                                        notExitsTableAction.current?.reload();
                                                        previewFormAction.current?.reload();
                                                    } else {
                                                        message.error(res.message || '移出失败');
                                                    }
                                                })
                                            }
                                        })
                                    }}
                                >移出</Button>,
                            ]}
                        />
                    </Card>
                </Col>
                <Col span={12}>
                    <Card bordered={true} bodyStyle={{ padding: 0 }}>
                        <ProTable<ModelField>
                            headerTitle="未加入字段"
                            options={{
                                fullScreen: false,
                                setting: false,
                            }}
                            actionRef={notExitsTableAction}
                            request={(params, sort, query) => {
                                params['ModelNum'] = column?.modelNum;
                                params['notColumnNum'] = column?.num;
                                return fieldPage({
                                    params,
                                    sort,
                                    query,
                                })
                            }}
                            columns={notExitsColumns}
                            rowSelection={{}}
                            toolBarRender={(action, { selectedRows }) => [
                                selectedRows && selectedRows.length > 0 &&
                                <Button
                                    icon={<SwapLeftOutlined />}
                                    type="primary"
                                    loading={loading.swapRight}
                                    onClick={() => {
                                        if (column) {
                                            setLoading({
                                                ...loading,
                                                swapLeft: true,
                                            })
                                            const fieldNum = selectedRows.map(temp => temp.num);
                                            MoveModelField(column.num, fieldNum).then(res => {
                                                setLoading({
                                                    ...loading,
                                                    swapLeft: false,
                                                })
                                                if (res.isSuccess) {
                                                    message.success('移入成功');
                                                    exitsTableAction.current?.reload();
                                                    notExitsTableAction.current?.reload();
                                                    notExitsTableAction.current?.clearSelected();
                                                    previewFormAction.current?.reload();
                                                } else {
                                                    message.error(res.message || '移入失败');
                                                }
                                            })
                                        }
                                    }}
                                >
                                    移入
                                </Button>,
                                <Button
                                    icon={<PlusOutlined />}
                                    type="primary"
                                    onClick={() => {
                                        setModelFieldAdd({
                                            visible: true,
                                            modelNum: column?.modelNum,
                                            editType: 'model'
                                        })
                                    }}
                                >添加</Button>,
                            ]}
                        />
                    </Card>
                </Col>
            </Row>
        </Drawer>
        <ModelFieldAdd
            {...modelFieldAdd}
            onClose={() => {
                setModelFieldAdd({
                    ...modelFieldAdd,
                    visible: false,
                })
            }}
            onSuccess={() => {
                notExitsTableAction.current?.reload();
                exitsTableAction.current?.reload();
                setModelFieldAdd({
                    ...modelFieldAdd,
                    visible: false,
                })
            }}
        />
        <Drawer
            title="预览表单"
            width="60%"
            visible={previewForm.visible}
            onClose={() => {
                setPreviewForm({
                    ...previewForm,
                    visible: false,
                })
            }}
        >
            <AsyncContentForm actionRef={previewFormAction} columnNum={previewForm.columnNum} />
        </Drawer>
    </>
}

export default ColumnFieldList;