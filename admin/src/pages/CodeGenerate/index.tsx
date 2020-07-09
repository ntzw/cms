import React, { useState, useEffect } from 'react';
import { Dispatch, connect } from 'umi';
import { Row, Col, Button, Card, Tree, Modal, Input, Form, Empty, message, Tooltip } from 'antd';
import EditorMd, { EditorMdInstance } from '@/components/Editor/EditorMd';
import { TreeProps } from 'antd/lib/tree';
import { PlusOutlined, SaveOutlined, DeleteOutlined, FormatPainterOutlined, FileMarkdownOutlined, FolderOpenOutlined } from '@ant-design/icons';
import IndexedDBUtils from '@/utils/IndexedDBUtils';
import Generate, { getEditrMdModeByFileName } from './components/Generate';
import BatchGenerate from './components/BatchGenerate';


interface CodeGenerateProps {
    dispatch: Dispatch;
}

export interface TemplateItem {
    title: string;
    key: string;
    parent: string;
    template?: string;
}

interface GenerateDrawerType {
    visible: boolean;
    template: string;
    templateName: string;
}

interface BatchGenerateModalType {
    visible: boolean;
    templateData?: TemplateItem[];
}

const layout = {
    labelCol: { span: 8 },
    wrapperCol: { span: 16 },
};

const dbApi = IndexedDBUtils('CodeGenerate');
const CodeGenerate: React.FC<CodeGenerateProps> = ({ dispatch }) => {
    const [editorMd] = useState(React.createRef<EditorMdInstance>());
    const [selectNodes, setSelectNodes] = useState<TreeProps['treeData']>([]);
    const [templateTreeData, setTemplateTreeData] = useState<TreeProps['treeData']>([]);
    const [templateTreeCheckData, setTemplateTreeCheckData] = useState<React.Key[]>();
    const [templateData, setTemplateData] = useState<TemplateItem[]>([]);

    const [editForm] = Form.useForm();
    const [generateDrawer, setGenerateDrawer] = useState<GenerateDrawerType>({
        visible: false,
        template: '',
        templateName: '',
    });

    const [batchGenerateModal, setBatchGenerateModal] = useState<BatchGenerateModalType>({
        visible: false,
    })
    const [editModal, setEditModal] = useState<{
        visible: boolean;
        key: string;
    }>({
        visible: false,
        key: ''
    });

    const getTreeData = (parentKey: string) => {
        const treeData: TreeProps['treeData'] = [];
        templateData.forEach(item => {
            if (item.parent === parentKey) {
                treeData.push({
                    title: item.title,
                    key: item.key,
                    children: getTreeData(item.key)
                })
            }
        })

        return treeData;
    }

    const setSelectTemplateText = () => {
        if (selectNodes && selectNodes.length > 0) {
            const info = templateData.find(item => item.key === selectNodes[0].key);
            editorMd.current?.setValue(info?.template || '');
            editorMd.current?.setCodeMirrorOption(getEditrMdModeByFileName(info?.title || ''))
        }
    }



    const tableName = 'templateData'
    useEffect(() => {
        dbApi.createTable(tableName, { keyPath: 'key' }).then(() => {
            dbApi.getAll<TemplateItem>(tableName).then(data => {
                setTemplateData(data);
            })
        });
       
        dispatch({
            type: 'codeGenerate/load'
        })
    }, []);


    useEffect(() => {
        templateData.forEach(temp => {
            dbApi.get(tableName, temp.key).then(data => {
                if (data) {
                    dbApi.update(tableName, temp)
                } else {
                    dbApi.update(tableName, temp);
                }
            })
        })

        const data = getTreeData('');
        setTemplateTreeData(data);
    }, [templateData])

    useEffect(() => {
        setSelectTemplateText();
    }, [selectNodes])

    return <>
        <Row>
            <Col span={8} style={{ padding: '0 2px 0 0' }}>
                <Card
                    title="模板列表"
                    extra={<Button.Group>
                        <Tooltip title="新增模板">
                            <Button
                                icon={<PlusOutlined />}
                                type="primary"
                                onClick={() => {
                                    setEditModal({
                                        visible: true,
                                        key: ''
                                    })
                                }}>
                            </Button>
                        </Tooltip>
                        <Tooltip title="删除模板">
                            <Button
                                icon={<DeleteOutlined />}
                                danger
                                type="primary"
                                disabled={!templateTreeCheckData || templateTreeCheckData.length <= 0}
                                onClick={() => {
                                    Modal.confirm({
                                        title: '删除提示',
                                        content: '确认删除当前勾选的模板？',
                                        onOk: () => {
                                            const data = templateData.filter(temp => templateTreeCheckData?.indexOf(temp.key) === -1);
                                            setTemplateData([...data]);

                                            templateTreeCheckData?.forEach(item => {
                                                dbApi.delete(tableName, item);
                                            })

                                            setTemplateTreeCheckData([]);
                                        }
                                    })
                                }}>
                            </Button>
                        </Tooltip>
                        <Tooltip title="批量生成">
                            <Button
                                icon={<FolderOpenOutlined />}
                                type="primary"
                                disabled={!templateTreeCheckData || templateTreeCheckData.length <= 0}
                                onClick={() => {
                                    setBatchGenerateModal({
                                        visible: true,
                                        templateData: templateData.filter(temp => templateTreeCheckData && templateTreeCheckData.indexOf(temp.key) > -1)
                                    })
                                }}>
                            </Button>
                        </Tooltip>
                    </Button.Group>}
                >
                    {templateTreeData && templateTreeData.length > 0 ? <Tree
                        checkable
                        showLine
                        showIcon
                        onCheck={(checked) => {
                            setTemplateTreeCheckData(checked as React.Key[]);
                        }}
                        // checkedKeys={checkedKeys}
                        selectedKeys={selectNodes?.map(temp => temp.key)}
                        onSelect={(selectedKeys, { selectedNodes }) => {
                            if (selectNodes && selectNodes.length > 0 && selectNodes[0].key !== selectedKeys[0]) {
                                Modal.confirm({
                                    title: '系统提示',
                                    content: '是否已保存当前编辑文档',
                                    okText: '已保存',
                                    cancelText: '取消',
                                    onOk: () => {
                                        setSelectNodes(selectedNodes)
                                    }
                                })
                            } else {
                                setSelectNodes(selectedNodes)
                            }
                        }}
                        treeData={templateTreeData}
                    /> : <Empty description="暂无模板数据" />}

                </Card>
            </Col>
            <Col span={16}>
                <Card
                    title="模板内容"
                    bodyStyle={{ padding: 0 }}
                    extra={<Button.Group>
                        <Tooltip title="保存">
                            <Button
                                icon={<SaveOutlined />}
                                type="primary"
                                disabled={!selectNodes || selectNodes.length <= 0}
                                onClick={() => {
                                    editorMd.current?.submit();
                                }}>
                            </Button>
                        </Tooltip>
                        <Tooltip title="生成">
                            <Button
                                icon={<FileMarkdownOutlined />}
                                type="primary"
                                disabled={!selectNodes || selectNodes.length <= 0}
                                onClick={() => {
                                    if (selectNodes && selectNodes.length > 0) {
                                        const info = templateData?.find(temp => temp.key === selectNodes[0].key);
                                        setGenerateDrawer({
                                            visible: true,
                                            template: info?.template || '',
                                            templateName: info?.title || '',
                                        })
                                    }
                                }}>
                            </Button>
                        </Tooltip>
                    </Button.Group>}
                >
                    {selectNodes && selectNodes.length > 0 ? <EditorMd
                        ref={editorMd}
                        style={{ width: '100%', height: 690, borderTop: '1px solid #f0f0f0' }}
                        config={{
                            watch: false,
                            toolbar: false,
                            codeFold: true,
                            searchReplace: true,
                            theme: 'default',
                            mode: 'text/html',
                            placeholder: '',
                        }}
                        onReady={() => {
                            setSelectTemplateText();
                        }}
                        onFinish={(value) => {
                            if (!value || !value.trim()) {
                                message.error('请填写模板内容');
                                return;
                            }

                            if (selectNodes && selectNodes.length > 0) {
                                const data = [...templateData];
                                const info = data.find(item => item.key === selectNodes[0].key);
                                if (info) {
                                    info.template = value;
                                }
                                setTemplateData(data);
                                message.success('模板内容保存成功！');
                            }
                        }}
                    /> : <Empty style={{ padding: 20 }} description="暂未选择模板" />}
                </Card>
            </Col>
        </Row>
        <Modal
            title={`${(editModal.key ? '编辑' : '添加')}模板`}
            visible={editModal.visible}
            onOk={() => {
                editForm.submit();
            }}
            onCancel={() => {
                setEditModal({
                    ...editModal,
                    visible: false,
                })
                editForm.resetFields();
            }}
        >
            <Form
                {...layout}
                name="basic"
                form={editForm}
                onFinish={(value) => {
                    const data = [...templateData];
                    if (editModal.key) {
                        const info = data.find(temp => temp.key === editModal.key);
                        if (info) {
                            info.title = value.title;
                        }
                    } else {
                        data.push({
                            title: value.title,
                            key: Date.now().toString(),
                            parent: selectNodes && selectNodes.length > 0 ? selectNodes[0].key.toString() : '',
                        })
                    }

                    setTemplateData(data);
                    setEditModal({
                        ...editModal,
                        visible: false,
                    })
                    editForm.resetFields();
                }}
            >
                <Form.Item
                    label="模板文件名"
                    name="title"
                    rules={[
                        { required: true, message: '请输入模板文件名' },
                    ]}
                >
                    <Input placeholder="输入模板文件名，带后缀" />
                </Form.Item>
            </Form>
        </Modal>
        <Generate
            {...generateDrawer}
            onClose={() => {
                setGenerateDrawer({
                    ...generateDrawer,
                    visible: false,
                })
            }}
        />
        <BatchGenerate
            {...batchGenerateModal}
            onClose={() => {
                setBatchGenerateModal({
                    ...batchGenerateModal,
                    visible: false,
                })
            }}
        />
    </>
}

export default connect()(CodeGenerate);
