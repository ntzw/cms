import ProTable, { ProColumns, ActionType, ProTableProps, ColumnsState, QuerySymbol, PageParamsType } from './Table';
import IndexColumn from './component/indexColumn';
import { RequestData } from './useFetchData';
import TableDropdown from './component/dropdown';
import TableStatus from './component/status';
import {
  IntlProvider,
  IntlConsumer,
  createIntl,
  IntlType,
  zhCNIntl,
  enUSIntl,
  viVNIntl,
  itITIntl,
  jaJPIntl,
  esESIntl,
  ruRUIntl,
  msMYIntl,
  zhTWIntl,
} from './component/intlContext';
import Search from './Form';
import defaultRenderText, { ProColumnsValueType } from './defaultRender';

export interface ModalBase {
  id: number;
  num: string;
}

export type {
  ProTableProps,
  IntlType,
  ColumnsState,
  ProColumnsValueType,
  ProColumns,
  ActionType,
  RequestData,
  PageParamsType,
};

export {
  IndexColumn,
  TableDropdown,
  TableStatus,
  Search,
  IntlProvider,
  IntlConsumer,
  zhCNIntl,
  defaultRenderText,
  createIntl,
  enUSIntl,
  viVNIntl,
  itITIntl,
  jaJPIntl,
  esESIntl,
  ruRUIntl,
  msMYIntl,
  zhTWIntl,
  QuerySymbol
};

export default ProTable;
