import { jwtDecode } from "jwt-decode";
import Cookies from 'js-cookie';
import { GetApiUrl } from '@constants/api'
let getApiUrl = GetApiUrl()
// Hàm đọc accesstoken
export function ReadToken(token) {
    if (token) {
        const decoded = jwtDecode(token)
        return {
            IdUser: decoded.sub,
            Phone: decoded.PhoneNumber,
            Name: decoded.FullName,
            Role: decoded.role,
            Exp: decoded.exp // Đơn vị giây
        }
    }
    else{
        return null
    }
}

// ValidateToken
export async function ValidateToken(accessToken, refreshToken) {
    if (accessToken && refreshToken) {
        try {
            var readtoken = ReadToken(accessToken)
            var exp = readtoken.Exp;
            var current = Math.floor(Date.now() / 1000); // quy đổi mili giây sang giây
            //Nếu token hết hạn, làm mới token mới
            if (exp < current) {
                const content = {
                    id: readtoken.IdUser,
                    hoTen: readtoken.Name,
                    sdt: readtoken.Phone,
                    vaiTro: readtoken.Role,
                    refreshToken: refreshToken,
                }
                const response = await fetch(getApiUrl+`/api/Account/RenewAccessToken`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(content)
                })
                if (!response.ok) {
                    const errorData = await response.text();    
                    throw new Error(`Lỗi ${response.status}: ${errorData || 'Không thể làm mới token'}`);
                }
                const result = await response.json()
                if (result.success) {
                    Cookies.set('accessToken', result.data.accessToken, { expires: 3 / 24 });
                    return true;
                } else {
                    Cookies.remove('accessToken', { path: '/' });
                    Cookies.remove('refreshToken', { path: '/' });
                    return false;
                }
            }
            return true;
        } catch (error) {
            console.log('Lỗi', error)
        }
    }else{
        Cookies.remove('accessToken', { path: '/' });
        Cookies.remove('refreshToken', { path: '/' });
        return false;
    }
}