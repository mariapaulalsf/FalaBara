const API_URL = "http://localhost:5282/api";

async function login() {
    const email = document.getElementById("email").value;
    const senha = document.getElementById("senha").value;

    if (!email || !senha) {
        alert("Por favor, preencha todos os campos");
        return;
    }

    try {
        const resposta = await fetch(`${API_URL}/auth/login`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                email: email,
                senha: senha
            })
        });

        const dados = await resposta.json();

        if (resposta.ok) {
            localStorage.setItem("token", dados.token);
            localStorage.setItem("usuario", JSON.stringify({
                nome: dados.nome,
                email: dados.email
            }));
            alert("Login realizado com sucesso!");
            window.location.href = "index.html";
        } else {
            alert(dados.mensagem || "Erro ao fazer login");
        }
    } catch (erro) {
        console.error("Erro:", erro);
        alert("Erro ao conectar com o servidor");
    }
}
