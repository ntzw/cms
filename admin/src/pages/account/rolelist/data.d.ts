import { ModalBase } from '@/components/ListTable';

export interface RoleListProps {

}

export interface Role extends ModalBase {
    parentNum: stirng | string[];
    children?: Role[];
}