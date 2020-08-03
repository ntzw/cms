import React, { useState, useEffect, createRef } from 'react';
import { ModelFieldAddProps } from "../data";
import { Modal, Form, Input, Select, Switch, InputNumber, message, Spin, Radio } from "antd";
import { filterEnumKey } from '@/utils/utils';
import { SubmitModelField, GetColumnFieldEditValue, GetModelFieldEditValue, SubmitColumnFieldEdit } from '../service';
import { FormItemType, GetFormItemTypeName } from '@/components/DynamicForm';
import { HandleResult } from '@/utils/request';
import { FormInstance } from 'antd/lib/form';
import { FieldDefaultType } from '@/components/Content/data';
import { SelectProps } from 'antd/lib/select';

const layout = {
    labelCol: { span: 6 },
    wrapperCol: { span: 16 },
};

const RequiredSwitch = ({ key = 'required' }: { key?: string }) => {
    return <Form.Item
        key={key}
        name="required"
        label="是否必须"
        valuePropName="checked"
    >
        <Switch checkedChildren="是" unCheckedChildren="否" />
    </Form.Item>
}

const ExtraInput = ({ key = 'extra' }: { key?: string }) => {
    return <Form.Item
        key={key}
        name="extra"
        label="提示信息"
    >
        <Input />
    </Form.Item>
}

const PasswordOption = () => {
    return <Form.Item
        key="maxLength"
        name="maxLength"
        label="字符长度"
    >
        <InputNumber min={0} max={100} />
    </Form.Item>
}


const InputOption = () => {
    return <>
        <Form.Item
            key="maxLength"
            name="maxLength"
            label="字符长度"
        >
            <InputNumber min={0} max={4000} />
        </Form.Item>
        <Form.Item
            key="addonBefore"
            name="addonBefore"
            label="设置前置标签"
        >
            <Input />
        </Form.Item>
        <Form.Item
            key="addonAfter"
            name="addonAfter"
            label="设置后置标签"
        >
            <Input />
        </Form.Item>
        <Form.Item
            key="regularTypes"
            name="regularTypes"
            label="验证类型"
        >
            <Select mode="multiple">
                <Select.Option value="1">手机号</Select.Option>
            </Select>
        </Form.Item>
    </>
}

export const SelectDataSource: SelectProps<any>['options'] = [{
    label: '自定义',
    value: 'custom',
}, {
    label: '当前栏目',
    value: 'currentColumn'
}];
const SelectOption: React.FC<{
    oldValue?: FieldDefaultType;
    form: React.RefObject<FormInstance>;
}> = ({
    oldValue,
    form
}) => {
        const [currentDataSource, setCurrentDataSource] = useState(SelectDataSource[0].value);
        useEffect(() => {
            if (oldValue) {
                const options = JSON.parse(oldValue.options || '{}');
                if (options.labelFieldName) {
                    setCurrentDataSource(options.labelFieldName);
                }
            }
        }, [oldValue]);

        const renderDataSourceOptions = () => {
            switch (currentDataSource) {
                case SelectDataSource[0].value:
                    return <>
                        <Form.Item
                            key="options"
                            name="options"
                            label="选项内容"
                        >
                            <Select mode="tags" placeholder="输入选项内容后回车，支持多个选项" />
                        </Form.Item>
                        <Form.Item
                            key="multiple"
                            name="multiple"
                            label="是否允许多选"
                            valuePropName="checked"
                        >
                            <Switch checkedChildren="是" unCheckedChildren="否" />
                        </Form.Item>
                    </>
                case SelectDataSource[1].value:
                    return <>
                        <Form.Item
                            key="labelFieldName"
                            name="labelFieldName"
                            label="显示字段名称"
                            rules={[{ required: true, message: '请填写显示字段名称' }]}
                        >
                            <Input />
                        </Form.Item>
                    </>
                default:
                    return <></>
            }
        }

        return <>
            <Form.Item
                key="dataSource"
                name="dataSource"
                label="数据源"
            >
                <Select
                    options={SelectDataSource}
                    onChange={(value) => {
                        setCurrentDataSource(value);
                    }}
                />
            </Form.Item>
            {renderDataSourceOptions()}
        </>
    }

const imageAccepts = ['.jpg', '.jpeg', '.png', '.gif', '.bmp'];
const fileAccepts = ['.pdf', '.doc', '.docx', '.xls', '.xlsx'];
const UploadOption: React.FC<{
    oldValue?: FieldDefaultType;
    form: React.RefObject<FormInstance>;
}> = ({
    oldValue,
    form
}) => {
        const [currentUploadType, setCurrentUploadType] = useState('image');
        const [accepts, setAccepts] = useState<string[]>([]);

        useEffect(() => {
            if (oldValue) {
                const options = JSON.parse(oldValue.options || '{}');
                if (options.uploadType) {
                    setCurrentUploadType(options.uploadType);
                }
            }
        }, [oldValue]);

        useEffect(() => {
            switch (currentUploadType) {
                case 'image':
                    setAccepts(imageAccepts);
                    break;
                case 'file':
                    setAccepts([...fileAccepts, ...imageAccepts]);
                    break;
            }
        }, [currentUploadType])

        return <>
            <Form.Item
                key="uploadType"
                name="uploadType"
                label="上传类型"
            >
                <Radio.Group
                    onChange={(e) => {
                        const tempType = e.target.value;
                        setCurrentUploadType(tempType);

                        const options = JSON.parse(oldValue?.options || '{}');
                        if (options.uploadType !== tempType) {
                            form.current?.setFieldsValue({
                                accept: tempType === 'image' ? imageAccepts : fileAccepts
                            })
                        } else {
                            form.current?.setFieldsValue({
                                accept: options.accept || []
                            })
                        }
                    }}
                >
                    <Radio value="image">图片</Radio>
                    <Radio value="file">文件</Radio>
                </Radio.Group>
            </Form.Item>
            <Form.Item
                key="uploadMax"
                name="uploadMax"
                label="允许上传数量"
            >
                <InputNumber min={0} max={20} />
            </Form.Item>
            <Form.Item
                key="accept"
                name="accept"
                label="允许文件类型"
                rules={[{ required: true, message: '请至少选择一个文件类型' }]}
            >
                <Select
                    mode="multiple"
                    allowClear
                    options={accepts.map(temp => ({
                        label: temp,
                        value: temp,
                    }))}
                />
            </Form.Item>
        </>
    }

const SwitchOptions = () => {
    return <>
        <Form.Item
            key="checkedChildren"
            name="checkedChildren"
            label="选中时的内容"
        >
            <Input />
        </Form.Item>
        <Form.Item
            key="unCheckedChildren"
            name="unCheckedChildren"
            label="非选中时的内容"
        >
            <Input />
        </Form.Item>
    </>
}

export const CascaderDataSource: SelectProps<any>['options'] = [{
    label: '城市',
    value: 'region'
}, {
    label: '当前栏目',
    value: 'currentColumn'
}];

const CascaderOptions: React.FC<{
    oldValue?: FieldDefaultType;
    form: React.RefObject<FormInstance>;
}> = ({
    oldValue,
    form
}) => {

        const [currentDataSource, setCurrentDataSource] = useState(CascaderDataSource[0].value);
        useEffect(() => {
            if (oldValue) {
                const options = JSON.parse(oldValue.options || '{}');
                if (options.dataSource) {
                    setCurrentDataSource(options.dataSource);
                }
            }
        }, [oldValue]);

        const renderDataSourceOptions = () => {
            switch (currentDataSource) {
                case CascaderDataSource[0].value:
                    return <></>
                case CascaderDataSource[1].value:
                    return <>
                        <Form.Item
                            key="labelFieldName"
                            name="labelFieldName"
                            label="显示字段名称"
                            rules={[{ required: true, message: '请填写显示字段名称' }]}
                        >
                            <Input />
                        </Form.Item>
                    </>
                default:
                    return <></>
            }
        }

        return <>
            <Form.Item
                key="changeOnSelect"
                name="changeOnSelect"
                label="是否每层可选"
                valuePropName="checked"
            >
                <Switch checkedChildren="是" unCheckedChildren="否" />
            </Form.Item>
            <Form.Item
                key="dataSource"
                name="dataSource"
                label="数据源"
            >
                <Select
                    options={CascaderDataSource}
                    onChange={(value) => {
                        setCurrentDataSource(value);
                    }}
                />
            </Form.Item>
            {renderDataSourceOptions()}
        </>
    }

const FormFieldOptions: React.FC<{
    optionType: FormItemType;
    form: React.RefObject<FormInstance>;
    oldValue?: FieldDefaultType;
}> = ({ optionType, form, oldValue }) => {
    switch (optionType) {
        case FormItemType.password:
            return <PasswordOption />;
        case FormItemType.input:
        case FormItemType.textArea:
            return <InputOption />;
        case FormItemType.select:
            return <SelectOption form={form} oldValue={oldValue} />;
        case FormItemType.upload:
            return <UploadOption form={form} oldValue={oldValue} />;
        case FormItemType.switch:
            return <SwitchOptions />;
        case FormItemType.cascader:
            return <CascaderOptions form={form} oldValue={oldValue} />
        default:
            return <></>;
    }
}


const ModelFieldAdd: React.FC<ModelFieldAddProps> = ({
    visible,
    onClose,
    modelNum,
    onSuccess,
    editType,
    editId,
}) => {
    const [form] = useState(createRef<FormInstance>());
    const [currentOptionType, setCurrentOptionType] = useState('');
    const [isUpdate, setIsUpdate] = useState(false);
    const [oldValue, setOldValue] = useState<FieldDefaultType>();
    const [loading, setLoading] = useState({
        submit: false,
        loadFields: false,
    })

    const getValueCallback = (res: HandleResult<FieldDefaultType>) => {
        if (res.isSuccess && res.data) {
            setOldValue(res.data);

            const { name, explain, optionType, options } = res.data;

            const tempOptions = JSON.parse(options);
            setCurrentOptionType(String(optionType));
            form.current?.setFieldsValue({
                name,
                explain,
                optionType: String(optionType),
                ...tempOptions,
            })
        }

        setLoading({
            ...loading,
            loadFields: false,
        })
    }

    useEffect(() => {
        form.current?.resetFields();
        if (visible && editId && editType) {
            setLoading({
                ...loading,
                loadFields: true,
            })

            switch (editType) {
                case 'column':
                    GetColumnFieldEditValue(editId).then(getValueCallback)
                    break;
                case 'model':
                    GetModelFieldEditValue(editId).then(getValueCallback)
                    break;
            }
        }

        setIsUpdate(!!editId && editId > 0);
    }, [visible])

    return <Modal
        title="添加字段"
        visible={visible}
        onCancel={() => {
            onClose();
        }}
        centered={true}
        confirmLoading={loading.submit}
        onOk={() => {
            form.current?.submit();
        }}
    >
        <Spin
            spinning={loading.loadFields}
            tip="表单生成中，请稍后...."
        >
            <Form
                {...layout}
                ref={form}
                name="ModelFieldAdd"
                initialValues={{
                    maxLength: 500,
                    uploadType: 'image',
                    uploadMax: 1,
                    accept: imageAccepts,
                    checkedChildren: '是',
                    unCheckedChildren: '否',
                }}
                onFinish={(value) => {
                    setLoading({
                        ...loading,
                        submit: true,
                    })

                    const { name, explain, optionType, ...options } = value;
                    const editValue = {
                        id: editId,
                        name,
                        explain,
                        optionType,
                        modelNum,
                        options: JSON.stringify(options)
                    }

                    const submitResult = (res: HandleResult<any>): void => {
                        if (res.isSuccess) {
                            onSuccess();
                        } else {
                            message.error(res.message || '操作失败');
                        }

                        setLoading({
                            ...loading,
                            submit: false,
                        });
                    };

                    if (editType === 'model') {
                        SubmitModelField(editValue).then(submitResult)
                    } else {
                        SubmitColumnFieldEdit(editValue).then(submitResult);
                    }
                }}
            >
                <Form.Item
                    name="name"
                    label="名称"
                    extra={<span style={{ color: 'green' }}>名称一经添加将不允许修改，请谨慎填写！</span>}
                    rules={[{ required: true, message: "请填写名称" }, { pattern: /^([a-zA-Z]{1})([a-zA-Z0-9]{2,})$/, message: "名称首字母必须是英文，且仅支持英文字母和数字，并且至少三个字符" }]}>
                    <Input maxLength={50} disabled={isUpdate} />
                </Form.Item>
                <Form.Item extra={<span style={{ color: 'green' }}>操作类型一经添加将不允许修改，请谨慎选择！</span>} name="optionType" label="操作类型" rules={[{ required: true }]}>
                    <Select disabled={isUpdate} onChange={(value) => { setCurrentOptionType(value.toString()) }}>
                        {filterEnumKey(FormItemType).map(type => {
                            return <Select.Option key={type} value={type}>
                                {GetFormItemTypeName(Number(type))}
                            </Select.Option>
                        })}
                    </Select>
                </Form.Item>
                <Form.Item name="explain" label="说明" rules={[{ required: true }]}>
                    <Input maxLength={50} />
                </Form.Item>
                <RequiredSwitch />
                <ExtraInput />
                <FormFieldOptions
                    optionType={Number(currentOptionType)}
                    form={form}
                    oldValue={oldValue}
                />
            </Form>
        </Spin>
    </Modal>
}

export default ModelFieldAdd;