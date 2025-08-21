import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import { IncidentList } from '../incident-list';
import type { IncidentDto } from '@/lib/api-types';

// Mock data
const mockIncidents: IncidentDto[] = [
  {
    id: 1,
    title: '配送遅延トラブル',
    description: '商品の配送が予定より2日遅れている',
    category: '配送',
    status: 'Open',
    priority: 'Medium',
    reportedDate: '2025-01-15T10:00:00Z',
    assignedDate: null,
    resolvedDate: null,
    resolution: null,
    reportedById: 1,
    reportedBy: '田中太郎',
    assignedToId: null,
    assignedTo: null,
    createdAt: '2025-01-15T10:00:00Z',
    updatedAt: '2025-01-15T10:00:00Z'
  },
  {
    id: 2,
    title: '商品破損トラブル',
    description: '配送中に商品が破損した',
    category: '品質',
    status: 'Resolved',
    priority: 'High',
    reportedDate: '2025-01-14T09:00:00Z',
    assignedDate: '2025-01-14T10:00:00Z',
    resolvedDate: '2025-01-15T16:00:00Z',
    resolution: '商品を交換し、再配送を手配した',
    reportedById: 1,
    reportedBy: '田中太郎',
    assignedToId: 2,
    assignedTo: '佐藤花子',
    createdAt: '2025-01-14T09:00:00Z',
    updatedAt: '2025-01-15T16:00:00Z'
  }
];

const mockOnEdit = jest.fn();
const mockOnDelete = jest.fn();

describe('IncidentList', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('renders incident list correctly', () => {
    render(
      <IncidentList
        incidents={mockIncidents}
        onEdit={mockOnEdit}
        onDelete={mockOnDelete}
      />
    );

    // インシデントのタイトルが表示されることを確認
    expect(screen.getByText('配送遅延トラブル')).toBeInTheDocument();
    expect(screen.getByText('商品破損トラブル')).toBeInTheDocument();

    // インシデントの説明が表示されることを確認
    expect(screen.getByText('商品の配送が予定より2日遅れている')).toBeInTheDocument();
    expect(screen.getByText('配送中に商品が破損した')).toBeInTheDocument();

    // カテゴリが表示されることを確認
    expect(screen.getByText('配送')).toBeInTheDocument();
    expect(screen.getByText('品質')).toBeInTheDocument();
  });

  it('shows loading state when loading is true', () => {
    render(
      <IncidentList
        incidents={[]}
        onEdit={mockOnEdit}
        onDelete={mockOnDelete}
        loading={true}
      />
    );

    // ローディングスピナーが表示されることを確認
    expect(screen.getByRole('status')).toBeInTheDocument();
  });

  it('shows no data message when incidents array is empty', () => {
    render(
      <IncidentList
        incidents={[]}
        onEdit={mockOnEdit}
        onDelete={mockOnDelete}
        loading={false}
      />
    );

    // データがない場合のメッセージが表示されることを確認
    expect(screen.getByText('インシデントデータがありません')).toBeInTheDocument();
  });

  it('calls onEdit when edit button is clicked', () => {
    render(
      <IncidentList
        incidents={mockIncidents}
        onEdit={mockOnEdit}
        onDelete={mockOnDelete}
      />
    );

    // 最初のインシデントの編集ボタンをクリック
    const editButtons = screen.getAllByText('編集');
    fireEvent.click(editButtons[0]);

    // onEditが正しいインシデントで呼ばれることを確認
    expect(mockOnEdit).toHaveBeenCalledWith(mockIncidents[0]);
  });

  it('calls onDelete when delete button is clicked', () => {
    render(
      <IncidentList
        incidents={mockIncidents}
        onEdit={mockOnEdit}
        onDelete={mockOnDelete}
      />
    );

    // 最初のインシデントの削除ボタンをクリック
    const deleteButtons = screen.getAllByText('削除');
    fireEvent.click(deleteButtons[0]);

    // onDeleteが正しいインシデントで呼ばれることを確認
    expect(mockOnDelete).toHaveBeenCalledWith(mockIncidents[0]);
  });

  it('displays correct status badges', () => {
    render(
      <IncidentList
        incidents={mockIncidents}
        onEdit={mockOnEdit}
        onDelete={mockOnDelete}
      />
    );

    // ステータスバッジが表示されることを確認
    expect(screen.getByText('オープン')).toBeInTheDocument();
    expect(screen.getByText('解決済み')).toBeInTheDocument();
  });

  it('displays correct priority badges', () => {
    render(
      <IncidentList
        incidents={mockIncidents}
        onEdit={mockOnEdit}
        onDelete={mockOnDelete}
      />
    );

    // 優先度バッジが表示されることを確認
    expect(screen.getByText('中')).toBeInTheDocument();
    expect(screen.getByText('高')).toBeInTheDocument();
  });

  it('displays assigned user when available', () => {
    render(
      <IncidentList
        incidents={mockIncidents}
        onEdit={mockOnEdit}
        onDelete={mockOnDelete}
      />
    );

    // 割り当てられたユーザーが表示されることを確認
    expect(screen.getByText('佐藤花子')).toBeInTheDocument();
  });

  it('displays resolution when available', () => {
    render(
      <IncidentList
        incidents={mockIncidents}
        onEdit={mockOnEdit}
        onDelete={mockOnDelete}
      />
    );

    // 解決策が表示されることを確認
    expect(screen.getByText('商品を交換し、再配送を手配した')).toBeInTheDocument();
  });

  it('formats dates correctly', () => {
    render(
      <IncidentList
        incidents={mockIncidents}
        onEdit={mockOnEdit}
        onDelete={mockOnDelete}
      />
    );

    // 日付が正しくフォーマットされて表示されることを確認
    expect(screen.getByText(/2025\/01\/15/)).toBeInTheDocument();
    expect(screen.getByText(/2025\/01\/14/)).toBeInTheDocument();
  });
});
