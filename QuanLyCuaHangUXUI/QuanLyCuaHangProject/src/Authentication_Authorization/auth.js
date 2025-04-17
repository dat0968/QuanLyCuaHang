import { jwtDecode } from "jwt-decode";
import Cookies from 'js-cookie';
import { GetApiUrl } from '@constants/api'
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
                    readtoken: refreshToken,
                }
                const response = await fetch(GetApiUrl()+`/api/Account/RenewAccessToken`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(content)
                })
                if (!response.ok) {
                    throw new Error('Lỗi ' + response.status)
                }
                const result = await response.json()
                if (result.success) {
                    Cookies.set('accessToken', result.data.accessToken, { expires: 3 / 24 });
                    return true;
                } else {
                    return false;
                }
            }
            return true;
        } catch (error) {
            console.log('Lỗi', error)
        }
    }else{
        return false;
    }
}