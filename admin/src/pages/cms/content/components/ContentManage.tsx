import React, { useRef, useState, useEffect, Suspense } from 'react';
import { ContentManageProps, ColumnContentItem, ColumnItem, ContentEditState, CategoryManagementState } from '../data';
import ProTable, { ActionType as TableAction, ProColumns } from '@/components/ListTable';
import { ContentPage, GetEditValue, ContentSubmit, SubmitContentTopStatus, ContentDelete, ContentMoveRecycle } from '../service';
import { ColumnField, ContentFormAction } from '@/components/Content/data';
import { lowerCaseFieldName } from '@/utils/utils';
import moment from 'moment';
import { Button, Tooltip, Spin, message, Switch, Modal } from 'antd';
import { EditOutlined, PlusOutlined, BarsOutlined, DeleteOutlined, RestOutlined, MenuOutlined } from '@ant-design/icons';
import ContentEditDrawer from './ContentEditDrawer';
import CategoryManagement from './CategoryManagement';
import ContentForm from '@/components/Content/ContentForm';
import { Column } from '../../columnlist/data';
import { HandleResult } from '@/utils/request';
import { DeleteConfirm } from '@/utils/msg';
import { PageLoading } from '@ant-design/pro-layout';
import RecycleList from './RecycleList';
import SeoForm from '@/components/Content/SeoForm';


const ContentListTable: React.FC<{
    currentColumn?: ColumnItem;
    currentColumnNum?: string;
    currentTableFields?: ColumnField[];
}> = ({
    currentColumn,
    currentColumnNum,
    currentTableFields
}) => {
        const tableAction = useRef<TableAction>();
        const [contentTableColumns, setContentTableColumns] = useState<ProColumns<ColumnContentItem>[]>([]);
        const [contentEdit, setContentEdit] = useState<ContentEditState>({
            visible: false,
        });
        const [categoryManage, setCategoryManage] = useState<CategoryManagementState>({
            visible: false,
        });
        const [recycleDrawer, setRecycleDrawer] = useState(false);
        const [loadColumns, setLoadColumns] = useState(false);
        const [editDataIsSuccess, setEditDataIsSuccess] = useState(false);
        const [seoForm, setSeoForm] = useState<{
            visible: boolean;
            getDataUrl: string;
            setDataUrl: string;
            param?: { [key: string]: any };
        }>({
            visible: false,
            getDataUrl: '',
            setDataUrl: '',
        });
        const contentAction = useRef<ContentFormAction>();

        const fieldsToColumns = (currentTableFields: ColumnField[], currentColumn: ColumnItem | undefined, tableAction: React.MutableRefObject<TableAction | undefined>) => {
            const columns: ProColumns<ColumnContentItem>[] = currentTableFields.map((temp): ProColumns<ColumnContentItem> => {
                return {
                    dataIndex: lowerCaseFieldName(temp.name),
                    title: temp.explain,
                    ellipsis: true,
                    width: 100,
                };
            });

            if (currentColumn?.isAllowTop) {
                columns.push({
                    dataIndex: 'isTop',
                    title: '是否置顶',
                    render: (_, row) => {
                        return <Switch
                            checked={row.isTop}
                            checkedChildren="已置顶"
                            unCheckedChildren="未置顶"
                            onChange={(isTop: boolean) => {
                                SubmitContentTopStatus(row.num, row.columnNum, isTop).then(res => {
                                    if (res.isSuccess) {
                                        message.success('置顶状态修改成功');
                                        tableAction.current?.reload();
                                    }
                                    else {
                                        message.error('置顶状态修改失败');
                                    }
                                });
                            }} />;
                    }
                });
            }

            columns.push({
                dataIndex: 'createDate',
                title: '创建时间',
                render: (value) => {
                    return moment(value + '').format('YYYY-MM-DD HH:mm');
                }
            });

            columns.push({
                dataIndex: '_',
                title: '操作',
                render: (_, row) => {
                    return <Button.Group>
                        <Tooltip title="编辑">
                            <Button
                                icon={<EditOutlined />}
                                type="primary"
                                onClick={() => {
                                    setEditDataIsSuccess(false);
                                    setContentEdit({
                                        visible: true,
                                        itemNum: row.num,
                                    });
                                }} />
                        </Tooltip>
                        <Tooltip title="SEO设置">
                            <Button
                                icon={<MenuOutlined />}
                                type="primary"
                                onClick={() => {
                                    setSeoForm({
                                        visible: true,
                                        getDataUrl: '/Api/CMS/Content/GetSeo',
                                        setDataUrl: '/Api/CMS/Content/UpdateSeo',
                                        param: {
                                            id: row.id,
                                            num: row.num,
                                            columnNum: currentColumn?.num
                                        }
                                    })
                                }} />
                        </Tooltip>
                    </Button.Group>;
                }
            });
            return columns;
        }

        useEffect(() => {
            if (currentTableFields && currentTableFields.length > 0) {
                setLoadColumns(true);
                const columns: ProColumns<ColumnContentItem>[] = fieldsToColumns(currentTableFields, currentColumn, tableAction);
                setContentTableColumns(columns);
                tableAction.current?.reload();
                setLoadColumns(false);
            }
        }, [currentTableFields])


        return <Spin spinning={loadColumns}>
            <ProTable<ColumnContentItem>
                headerTitle={`栏目[${currentColumn?.name}] 内容列表`}
                actionRef={tableAction}
                request={(params, sort, query) => {
                    params['columnNum'] = currentColumnNum;
                    return ContentPage({
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
                params={{

                }}
                toolBarRender={(action, { selectedRowKeys, selectedRows }) => [
                    <Button
                        icon={<PlusOutlined />}
                        type="primary"
                        onClick={() => {
                            setEditDataIsSuccess(false);
                            setContentEdit({
                                visible: true,
                            })
                        }}
                    >新增</Button>,
                    selectedRows && selectedRows.length > 0 && <Button
                        icon={<DeleteOutlined />}
                        type="primary"
                        danger
                        onClick={() => {
                            const ids = selectedRows?.map(item => item.id) || [];
                            const resCallback = (res: HandleResult) => {
                                if (res.isSuccess) {
                                    message.success('数据操作成功');
                                    tableAction.current?.reload();
                                    tableAction.current?.clearSelected();
                                } else {
                                    message.error('数据操作失败');
                                }
                            };

                            if (currentColumn?.isAllowRecycle) {
                                Modal.confirm({
                                    title: '系统提示',
                                    content: '确定将数据移入回收站？',
                                    okText: '确定',
                                    cancelText: '取消',
                                    onOk: () => {
                                        ContentMoveRecycle(ids, currentColumn?.num || '').then(resCallback);
                                    }
                                })
                            } else {
                                DeleteConfirm(() => {
                                    ContentDelete(ids, currentColumn?.num || '').then(resCallback);
                                })
                            }
                        }}
                    >
                        {currentColumn?.isAllowRecycle ? '移动到回收站' : '删除'}
                    </Button>,
                    currentColumn?.isCategory && <Button
                        icon={<BarsOutlined />}
                        onClick={() => {
                            setCategoryManage({
                                visible: true,
                            })
                        }}
                    >类别管理</Button>,
                    currentColumn?.isAllowRecycle && <Button
                        icon={<RestOutlined />}
                        onClick={() => {
                            setRecycleDrawer(true);
                        }}
                    >
                        查看回收站
                    </Button>,
                    <Button
                        type="primary"
                        onClick={() => {
                            setSeoForm({
                                visible: true,
                                getDataUrl: '/Api/CMS/Column/GetSeo',
                                setDataUrl: '/Api/CMS/Column/UpdateSeo',
                                param: {
                                    num: currentColumn?.num
                                }
                            })
                        }}
                    >栏目SEO设置</Button>
                ]}
            />
            <Suspense fallback={<PageLoading />}>
                <ContentEditDrawer
                    {...contentEdit}
                    actionRef={contentAction}
                    afterVisibleChange={(visible) => {
                        if (!visible && editDataIsSuccess) {
                            tableAction.current?.reload();
                        }
                    }}
                    onClose={(isSuccess) => {
                        setEditDataIsSuccess(isSuccess || false);
                        setContentEdit({
                            ...contentEdit,
                            visible: false
                        })
                    }}
                />
            </Suspense>
            <Suspense fallback={<PageLoading />}>
                <CategoryManagement
                    {...categoryManage}
                    onClose={() => {
                        contentAction.current?.reoladFieldItem();
                        setCategoryManage({
                            ...categoryManage,
                            visible: false,
                        })
                    }}
                />
            </Suspense>
            <Suspense fallback={<PageLoading />}>
                <RecycleList
                    visible={recycleDrawer}
                    onClose={() => { setRecycleDrawer(false); }}
                    afterVisibleChange={(visible) => {
                        if (!visible) {
                            tableAction.current?.reload();
                        }
                    }}
                    currentColumn={currentColumn}
                    currentTableFields={currentTableFields}
                />
            </Suspense>
            <Suspense fallback={<PageLoading />}>
                <SeoForm
                    {...seoForm}
                    onClose={() => {
                        setSeoForm({
                            ...seoForm,
                            visible: false,
                        })
                    }}
                />
            </Suspense>
        </Spin>
    }

const ContentSinageEdit: React.FC<{
    currentTableFields?: ColumnField[];
    currentColumnNum?: string;
    currentColumn?: Column;
}> = ({
    currentTableFields,
    currentColumnNum,
    currentColumn
}) => {
        const formAction = useRef<ContentFormAction>();
        const [submiting, setSubmiting] = useState(false);
        const [loadValue, setLoadValue] = useState(false);
        const [editValue, setEditValue] = useState<{ [key: string]: any }>();

        useEffect(() => {
            setLoadValue(true);
            GetEditValue('', currentColumnNum || '').then(res => {
                setEditValue(res.data || {});
                formAction.current?.setValue(res.data);

                setTimeout(() => {
                    setLoadValue(false);
                }, 500);
            })
        }, [currentColumnNum])

        return <div>
            <Spin spinning={loadValue} >
                <ContentForm
                    actionRef={formAction}
                    columnFields={currentTableFields || []}
                    columnNum={currentColumnNum || ''}
                    isSeo={currentColumn?.isSeo}
                    isCategory={currentColumn?.isCategory}
                    isAllowTop={currentColumn?.isAllowTop}
                    onFinish={(value) => {
                        setSubmiting(true);
                        return new Promise(resolve => {
                            const newValue = {
                                ...editValue,
                                ...value,
                                columnNum: currentColumnNum,
                            }

                            ContentSubmit(newValue).then(res => {
                                if (res.isSuccess) {
                                    message.success('数据提交成功');
                                } else {
                                    message.error(res.message || '数据提交失败');
                                }

                                resolve();
                                formAction.current?.reoladFieldItem();
                                setSubmiting(false);
                            })
                        })
                    }}
                />
                <div style={{ textAlign: 'center' }}>
                    <Button
                        type="primary"
                        loading={submiting}
                        onClick={() => {
                            formAction.current?.submit();
                        }}
                    >提交</Button>
                    <Button
                        danger
                        style={{ marginLeft: 5 }}
                        loading={submiting}
                        onClick={() => {
                            formAction.current?.clear();
                        }}
                    >重置</Button>
                </div>
            </Spin>
        </div>
    }


const ContentManage: React.FC<ContentManageProps> = ({
    currentColumn,
    currentColumnNum,
    currentTableFields
}) => {
    return currentColumn?.isSingle ?
        <ContentSinageEdit
            currentColumnNum={currentColumnNum}
            currentTableFields={currentTableFields}
            currentColumn={currentColumn}
        /> :
        <ContentListTable
            currentColumn={currentColumn}
            currentColumnNum={currentColumnNum}
            currentTableFields={currentTableFields}
        />
}

export default ContentManage;
