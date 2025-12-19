const API_URL = "http://localhost:5282/api";

async function registrar() {
    const nome = document.getElementById("nome").value;
    const email = document.getElementById("email").value;
    const senha = document.getElementById("senha").value;
    const confirmarSenha = document.getElementById("confirmarSenha").value;

    if (!nome || !email || !senha || !confirmarSenha) {
        alert("Por favor, preencha todos os campos");
        return;
    }

    if (senha !== confirmarSenha) {
        alert("As senhas não coincidem");
        return;
    }

    try {
        const resposta = await fetch(`${API_URL}/auth/registrar`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                nome: nome,
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
            alert("Registro realizado com sucesso!");
            window.location.href = "index.html";
        } else {
            alert(dados.mensagem || "Erro ao registrar");
        }
    } catch (erro) {
        console.error("Erro:", erro);
        alert("Erro ao conectar com o servidor");
    }
}
