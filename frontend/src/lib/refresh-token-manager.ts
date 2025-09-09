/**
 * リフレッシュトークン管理サービス
 * リフレッシュトークンの保存、取得、削除を管理
 */
export class RefreshTokenManager {
    private static readonly REFRESH_TOKEN_KEY = 'refresh_token';

    /**
     * リフレッシュトークンを保存
     * @param token リフレッシュトークン
     */
    static setRefreshToken(token: string): void {
        if (typeof window !== 'undefined') {
            localStorage.setItem(this.REFRESH_TOKEN_KEY, token);
        }
    }

    /**
     * リフレッシュトークンを取得
     * @returns リフレッシュトークンまたはnull
     */
    static getRefreshToken(): string | null {
        if (typeof window !== 'undefined') {
            return localStorage.getItem(this.REFRESH_TOKEN_KEY);
        }
        return null;
    }

    /**
     * リフレッシュトークンを削除
     */
    static clearRefreshToken(): void {
        if (typeof window !== 'undefined') {
            localStorage.removeItem(this.REFRESH_TOKEN_KEY);
        }
    }

    /**
     * リフレッシュトークンが存在するかチェック
     * @returns リフレッシュトークンが存在するかどうか
     */
    static hasRefreshToken(): boolean {
        return this.getRefreshToken() !== null;
    }
}
