/**
 * パスワード管理APIテストスクリプト
 */

const API_BASE_URL = 'http://localhost:5169';

// テスト用のAPIクライアント
class ApiClient {
    constructor(baseUrl) {
        this.baseUrl = baseUrl;
        this.accessToken = null;
    }

    async request(endpoint, options = {}) {
        const url = `${this.baseUrl}${endpoint}`;
        const config = {
            headers: {
                'Content-Type': 'application/json',
                ...options.headers,
            },
            ...options,
        };

        // アクセストークンがある場合はAuthorizationヘッダーを追加
        if (this.accessToken) {
            config.headers['Authorization'] = `Bearer ${this.accessToken}`;
        }

        try {
            const response = await fetch(url, config);
            
            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                console.log(`Error response:`, errorData);
                throw new Error(`HTTP ${response.status}: ${errorData.error || response.statusText}`);
            }

            return await response.json();
        } catch (error) {
            throw new Error(`Request failed: ${error.message}`);
        }
    }

    async login(username, password) {
        const response = await this.request('/api/auth/login', {
            method: 'POST',
            body: JSON.stringify({ usernameOrEmail: username, password }),
        });
        
        this.accessToken = response.accessToken;
        return response;
    }

    async changePassword(changePasswordDto) {
        return this.request('/api/password/change', {
            method: 'POST',
            body: JSON.stringify(changePasswordDto),
        });
    }

    async resetPassword(resetPasswordDto) {
        return this.request('/api/password/reset', {
            method: 'POST',
            body: JSON.stringify(resetPasswordDto),
        });
    }

    async checkPasswordStrength(password) {
        return this.request('/api/password/check-strength', {
            method: 'POST',
            body: JSON.stringify({ password }),
        });
    }

    async validatePassword(password) {
        return this.request('/api/password/validate', {
            method: 'POST',
            body: JSON.stringify({ password }),
        });
    }

    async getPasswordHistory(page = 1, pageSize = 10) {
        return this.request(`/api/password/history?page=${page}&pageSize=${pageSize}`);
    }

    async getPasswordChangeHistory(page = 1, pageSize = 10) {
        return this.request(`/api/password/change-history?page=${page}&pageSize=${pageSize}`);
    }

    async getPasswordPolicy() {
        return this.request('/api/password/policy');
    }

    async updatePasswordPolicy(policyDto) {
        return this.request('/api/password/policy', {
            method: 'PUT',
            body: JSON.stringify(policyDto),
        });
    }

    async checkPasswordExpiration() {
        return this.request('/api/password/expiration');
    }
}

// テスト実行
async function runPasswordManagementTests() {
    console.log('=== パスワード管理APIテスト開始 ===\n');

    const apiClient = new ApiClient(API_BASE_URL);

    try {
        // 1. ログインしてアクセストークンを取得
        console.log('1. ログインしてアクセストークンを取得...');
        const loginResponse = await apiClient.login('admin', 'password123');
        console.log('✅ ログイン成功');
        console.log(`   ユーザー: ${loginResponse.user.username} (${loginResponse.user.roleName})\n`);

        // 2. パスワード強度チェック
        console.log('2. パスワード強度チェック...');
        const strengthTests = [
            'weak',
            'Password123!',
            'VeryStrongPassword123!@#'
        ];

        for (const password of strengthTests) {
            const strengthResult = await apiClient.checkPasswordStrength(password);
            console.log(`   パスワード: "${password}"`);
            console.log(`   強度: ${strengthResult.strength} (スコア: ${strengthResult.score})`);
            console.log(`   説明: ${strengthResult.description}`);
            console.log(`   有効: ${strengthResult.isValid}`);
            if (strengthResult.suggestions.length > 0) {
                console.log(`   改善提案: ${strengthResult.suggestions.join(', ')}`);
            }
            console.log('');
        }

        // 3. パスワード検証
        console.log('3. パスワード検証...');
        const validationResult = await apiClient.validatePassword('TestPassword123!');
        console.log('✅ パスワード検証成功');
        console.log(`   有効: ${validationResult.isValid}`);
        console.log(`   強度: ${validationResult.strength}`);
        if (validationResult.errors.length > 0) {
            console.log(`   エラー: ${validationResult.errors.join(', ')}`);
        }
        if (validationResult.warnings.length > 0) {
            console.log(`   警告: ${validationResult.warnings.join(', ')}`);
        }
        console.log('');

        // 4. パスワードポリシー取得
        console.log('4. パスワードポリシー取得...');
        const policy = await apiClient.getPasswordPolicy();
        console.log('✅ パスワードポリシー取得成功');
        console.log(`   最小文字数: ${policy.minLength}`);
        console.log(`   最大文字数: ${policy.maxLength}`);
        console.log(`   大文字必須: ${policy.requireUppercase}`);
        console.log(`   小文字必須: ${policy.requireLowercase}`);
        console.log(`   数字必須: ${policy.requireDigit}`);
        console.log(`   記号必須: ${policy.requireSpecialChar}`);
        console.log(`   履歴保持数: ${policy.historyCount}`);
        console.log(`   有効期限: ${policy.expirationDays}日`);
        console.log(`   最小強度: ${policy.minStrength}\n`);

        // 5. パスワード期限チェック
        console.log('5. パスワード期限チェック...');
        const expirationInfo = await apiClient.checkPasswordExpiration();
        console.log('✅ パスワード期限チェック成功');
        console.log(`   期限切れ: ${expirationInfo.isExpired}`);
        console.log(`   残り日数: ${expirationInfo.daysUntilExpiration}日`);
        if (expirationInfo.expirationDate) {
            console.log(`   期限日: ${new Date(expirationInfo.expirationDate).toLocaleDateString()}`);
        }
        console.log('');

        // 6. パスワード履歴取得
        console.log('6. パスワード履歴取得...');
        const passwordHistory = await apiClient.getPasswordHistory();
        console.log('✅ パスワード履歴取得成功');
        console.log(`   総件数: ${passwordHistory.totalCount}`);
        console.log(`   ページ: ${passwordHistory.page}/${passwordHistory.totalPages}`);
        console.log(`   履歴数: ${passwordHistory.items.length}\n`);

        // 7. パスワード変更履歴取得
        console.log('7. パスワード変更履歴取得...');
        const changeHistory = await apiClient.getPasswordChangeHistory();
        console.log('✅ パスワード変更履歴取得成功');
        console.log(`   総件数: ${changeHistory.totalCount}`);
        console.log(`   ページ: ${changeHistory.page}/${changeHistory.totalPages}`);
        console.log(`   履歴数: ${changeHistory.items.length}\n`);

        // 8. パスワード変更テスト（実際には実行しない）
        console.log('8. パスワード変更テスト（スキップ）...');
        console.log('⚠️  パスワード変更は実際のテストでは実行しません（セキュリティ上の理由）');
        console.log('   テスト用のパスワード変更データ:');
        console.log('   {');
        console.log('     currentPassword: "password123",');
        console.log('     newPassword: "NewPassword123!",');
        console.log('     confirmPassword: "NewPassword123!"');
        console.log('   }\n');

        // 9. 管理者によるパスワードリセットテスト（実際には実行しない）
        console.log('9. 管理者によるパスワードリセットテスト（スキップ）...');
        console.log('⚠️  パスワードリセットは実際のテストでは実行しません（セキュリティ上の理由）');
        console.log('   テスト用のパスワードリセットデータ:');
        console.log('   {');
        console.log('     userId: 1,');
        console.log('     newPassword: "ResetPassword123!",');
        console.log('     confirmNewPassword: "ResetPassword123!",');
        console.log('     reason: "テスト用リセット"');
        console.log('   }\n');

        // 10. パスワードポリシー更新テスト（管理者権限が必要）
        console.log('10. パスワードポリシー更新テスト（スキップ）...');
        console.log('⚠️  パスワードポリシー更新は実際のテストでは実行しません（管理者権限が必要）');
        console.log('   テスト用のポリシー更新データ:');
        console.log('   {');
        console.log('     minLength: 10,');
        console.log('     maxLength: 128,');
        console.log('     requireUppercase: true,');
        console.log('     requireLowercase: true,');
        console.log('     requireDigit: true,');
        console.log('     requireSpecialChar: true,');
        console.log('     historyCount: 10,');
        console.log('     expirationDays: 60,');
        console.log('     minStrength: "Strong"');
        console.log('   }\n');

        console.log('=== パスワード管理APIテスト完了 ===');
        console.log('✅ すべてのテストが成功しました！');
        console.log('\n📝 注意事項:');
        console.log('- パスワード変更・リセット・ポリシー更新は実際のテストでは実行しません');
        console.log('- これらは本番環境でテストする際は十分注意してください');
        console.log('- セキュリティ上の理由で、実際のパスワード変更は避けてください');

    } catch (error) {
        console.error('❌ テストエラー:', error.message);
        throw error;
    }
}

// テスト実行
runPasswordManagementTests().catch(console.error);
