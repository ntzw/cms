import React, { useState, useEffect } from 'react';
import { Dispatch, connect, TableType, ModelState, TableField } from 'umi'
import { Drawer, Row, Col, Table, Empty, Spin, Button, message } from 'antd';
import EditorMd, { EditorMdInstance } from '@/components/Editor/EditorMd';
import artTemplate from "art-template/lib/template-web"
import jsZip from 'jszip'
import { saveAs } from 'file-saver'

interface GenerateProps {
    dispatch: Dispatch;
    tableData: TableType[];
    visible: boolean;
    template: string;
    templateName: string;
    onClose: () => void;
}

const getModalName = (tableName: string) => {
    const names = tableName.split('_');
    return names[names.length - 1];
}

export const buildTemplateValue = (template?: string, tableData?: TableType) => {
    if (template && tableData) {
        return artTemplate.render(template, {
            ModalName: getModalName(tableData.name),
            TableName: tableData.name,
            Fields: tableData.fields || []
        });
    }
    return '';
}

export const getEditrMdModeByFileName = (fileName: string) => {
    if (typeof fileName === 'string') {
        const temps = fileName.split('.');
        const ext = temps[temps.length - 1];

        switch (ext) {
            case 'html':
                return 'text/html';
            case 'cs':
                return 'text/x-csharp';
            default:
                return 'text/plain';
        }
    }

    return 'text/plain'
}


const Generate: React.FC<GenerateProps> = ({ visible, onClose, template, templateName, tableData }) => {
    const [editorMd] = useState(React.createRef<EditorMdInstance>());
    const [checkedRows, setCheckedRows] = useState<TableType[]>([]);
    const [selectedTable, setSelectedTable] = useState<TableType>();
    const [loading, setLoading] = useState({
        build: false,
        generate: false,
    })

    const setTemplateValue = () => {
        if (selectedTable && selectedTable.fields && selectedTable.fields.length > 0) {
            setLoading({
                ...loading,
                build: true,
            })

            const value = buildTemplateValue(template, selectedTable);
            if (value) {
                editorMd.current?.setValue(value || '');
                editorMd.current?.setCodeMirrorOption(getEditrMdModeByFileName(templateName));
                setLoading({
                    ...loading,
                    build: false,
                })
            }
        }
    }

    useEffect(() => {
        artTemplate.defaults.imports.SqlServerTypeFormat = (field: TableField) => {
            switch (field.dataType) {
                case 'int':
                    return 'int';
                case 'nvarchar':
                    return 'string';
                case 'datetime':
                    return 'DateTime';
                case 'bigint':
                    return 'long';
                case 'bit':
                    return 'bool';
                case 'decimal':
                    return 'decimal';
                default:
                    return field.dataType;
            }
        }
    }, []);

    useEffect(() => {
        if (visible) {
            setTemplateValue();
        }
    }, [visible])

    useEffect(() => {
        setTemplateValue();
    }, [selectedTable])

    return <Drawer
        title="生成代码"
        placement="left"
        width="90%"
        maskClosable={false}
        visible={visible}
        bodyStyle={{ padding: 5 }}
        onClose={onClose}
    >
        <Row>
            <Col span="6">
                <div>
                    <div>
                        <Button.Group>
                            <Button
                                type="primary"
                                loading={loading.generate}
                                disabled={!checkedRows || checkedRows.length <= 0}
                                onClick={() => {
                                    if (!checkedRows || checkedRows.length <= 0) {
                                        message.error('请选择要生成文件的表');
                                        return;
                                    }

                                    const names = templateName.split('.');
                                    const ext = names[names.length - 1];

                                    var zip = new jsZip();
                                    checkedRows.forEach(item => {
                                        const value = buildTemplateValue(template, item);
                                        zip.file(`${item.name}.${ext}`, value)
                                    })

                                    zip.generateAsync({ type: "blob" }).then(function (content) {
                                        // content就是blob数据，这里以example.zip名称下载    
                                        // 使用了FileSaver.js  
                                        saveAs(content, "example.zip");
                                    });
                                }}
                            >
                                生成
                            </Button>
                        </Button.Group>
                    </div>
                    <div style={{ marginTop: 5 }}>
                        <Table
                            bordered={true}
                            pagination={false}
                            size="small"
                            rowKey="name"
                            rowSelection={{
                                type: 'checkbox',
                                onChange: (selectedRowKeys, selectedRows) => {
                                    setCheckedRows(selectedRows);
                                },
                            }}
                            columns={[{
                                dataIndex: 'name',
                                title: '表名',
                            }]}
                            dataSource={tableData}
                            onRow={record => {
                                return {
                                    onClick: () => {
                                        setSelectedTable(record)
                                    }, // 点击行
                                };
                            }}
                        />
                    </div>
                </div>
            </Col>
            <Col
                span="18"
                style={{ paddingLeft: 5 }}
            >
                {selectedTable ? <Spin
                    tip="数据生成中...."
                    spinning={loading.build}
                >
                    <EditorMd
                        ref={editorMd}
                        style={{ width: '100%', height: 650 }}
                        config={{
                            watch: false,
                            toolbar: false,
                            codeFold: true,
                            searchReplace: true,
                            theme: 'default',
                            mode: 'text/html',
                            placeholder: '',
                            readOnly: true,
                        }}
                        onReady={() => {
                            setTemplateValue();
                        }}
                    />
                </Spin> : <Empty description="请选择要生成预览的表" />}
            </Col>
        </Row>
    </Drawer>
}

export default connect(({ codeGenerate: { tableData } }: { codeGenerate: ModelState }) => {
    return {
        tableData,
    }
})(Generate);